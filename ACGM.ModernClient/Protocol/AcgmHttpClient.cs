using System.Net;
using System.Net.Http.Headers;
using System.Text;
using ACGM.ModernClient.Models;
using ACGM.ModernClient.Logging;

namespace ACGM.ModernClient.Protocol;

public sealed class AcgmHttpClient : IDisposable
{
    private readonly HttpClient _client;
    private bool _disposed;

    public AcgmHttpClient(TimeSpan? timeout = null)
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        _client = new HttpClient(handler)
        {
            Timeout = timeout ?? TimeSpan.FromSeconds(30)
        };
        _client.DefaultRequestHeaders.UserAgent.ParseAdd(AcgmConstants.UserAgent);
        _client.DefaultRequestHeaders.ConnectionClose = true;
    }

    public async Task<AcgmResponse> PostAsync(Uri endpoint, LegacyPostEncoder postData, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(endpoint);
        ArgumentNullException.ThrowIfNull(postData);

        var body = postData.BuildBody();

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(body, Encoding.ASCII, "application/x-www-form-urlencoded")
        };

        // Match the old client as closely as possible. The old request supplied
        // User-Agent, Host, Connection: close and Content-length. HttpClient manages
        // Host and Content-Length while still negotiating TLS for https:// endpoints.
        request.Headers.UserAgent.Clear();
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("ACGM", AcgmConstants.LegacyVersionMajor));
        request.Headers.ConnectionClose = true;

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);

        var raw = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new InvalidOperationException("Server returned HTTP 404. Check the server.cgi path.");

        response.EnsureSuccessStatusCode();
        return AcgmResponse.Parse(raw);
    }

    public async Task<AcgmResponse> LoginAsync(Uri endpoint, string characterName, string password, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgLogin);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AcgmResponse> GetTreeAsync(Uri endpoint, string characterName, string password, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgGetTree);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AcgmResponse> GetCharacterInfoAsync(Uri endpoint, string characterName, string password, string characterId, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgGetCharInfo)
            .Add("getcharid", characterId);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }


    public async Task<AcgmResponse> UpdateCharacterInfoAsync(Uri endpoint, string characterName, string password, int loginCharacterId, int updateCharacterId, CharacterDetails original, CharacterDetails updated, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("charid", loginCharacterId)
            .Add("updateid", updateCharacterId)
            .Add("msgid", AcgmConstants.MsgUpdateCharInfo);

        AddChangedText(post, "NAME", original.Name, updated.Name);
        AddChangedText(post, "LEVEL", original.Level, updated.Level);
        AddChangedText(post, "CLASS", original.ClassName, updated.ClassName);
        AddChangedChoice(post, "RACE", original.Race, updated.Race);
        AddChangedChoice(post, "RANK", original.Rank, updated.Rank);
        AddChangedChoice(post, "SEX", original.Sex, updated.Sex);
        AddChangedBool(post, "ISMULE", original.IsMule, updated.IsMule);
        AddChangedText(post, "MULEFOR", original.MuleFor, updated.MuleFor);
        AddChangedText(post, "LSAT", original.LifestonedAt, updated.LifestonedAt);
        AddChangedText(post, "TIEDTO", original.TiedTo, updated.TiedTo);
        AddChangedText(post, "BIO", original.Bio, updated.Bio);
        AddChangedBool(post, "ISPK", original.IsPk, updated.IsPk);
        AddChangedBool(post, "ISRESCUE", original.IsRescueMember, updated.IsRescueMember);
        AddChangedBool(post, "ISPRIVATE", original.HideInfoOnWeb, updated.HideInfoOnWeb);
        AddChangedBool(post, "CAN_SUMMON", original.CanSummon, updated.CanSummon);
        AddChangedText(post, "MAIN_CHAR", original.MainCharacterName, updated.MainCharacterName);
        AddChangedText(post, "SKILLS", original.Skills, updated.Skills);
        AddChangedText(post, "REALNAME", original.RealName, updated.RealName);
        AddChangedText(post, "CITYSTATE", original.CityState, updated.CityState);
        AddChangedText(post, "MISCINFO", original.MiscRealLife, updated.MiscRealLife);
        AddChangedText(post, "EMAIL", original.Email, updated.Email);
        AddChangedText(post, "ICQ", original.Icq, updated.Icq);

        // VB6 SaveCharChanges always leaves the Awards/Admin-only field in the
        // legacy update path.  The CGI script treats ADMIN_ONLY as the final
        // saved character-detail field, so omitting it while saving another flag
        // can collapse or replace the legacy awards tail.  Preserve the loaded
        // Awards value even when the user did not edit the Awards tab.
        post.Add("ADMIN_ONLY", SerializeAwardsForLegacyPost(updated.Awards));

        var outgoingBody = AcgmDiagnostics.IsCharacterSaveCertificationEnabled ? post.BuildBody() : string.Empty;
        var response = await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
        AcgmDiagnostics.WriteCharacterSaveDiagnostics(original, updated, post.Fields, outgoingBody, response);
        return response;
    }


    public async Task<AcgmResponse> AddVassalAsync(Uri endpoint, string characterName, string password, int loginCharacterId, CharacterRecord targetPatron, string vassalName, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgAddVassal)
            .Add("charid", loginCharacterId)
            .Add("addtoid", targetPatron.Id)
            .Add("vassalname", vassalName)
            .Add("vassallist", targetPatron.VassalIds)
            .Add("patronid", targetPatron.PatronId);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AcgmResponse> RemoveVassalAsync(Uri endpoint, string characterName, string password, int loginCharacterId, CharacterRecord targetPatron, CharacterRecord vassal, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgRemoveVassal)
            .Add("charid", loginCharacterId)
            .Add("patronid", vassal.PatronId)
            .Add("vassalid", vassal.Id)
            .Add("vassalname", vassal.Name)
            .Add("vassallist", targetPatron.VassalIds);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AcgmResponse> ChangePatronAsync(Uri endpoint, string characterName, string password, int loginCharacterId, CharacterRecord movingCharacter, CharacterRecord? oldPatron, CharacterRecord newPatron, CancellationToken cancellationToken = default)
    {
        var oldPatronId = oldPatron?.Id ?? -1;
        var oldPatronName = oldPatron?.Name ?? "blah";
        var oldPatronVassalList = oldPatron?.VassalIds ?? "blah";

        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgChangePatron)
            .Add("moveid", movingCharacter.Id)
            .Add("charid", loginCharacterId)
            .Add("oldpatronid", oldPatronId)
            .Add("oldpatronname", oldPatronName)
            .Add("oldpatronvassallist", oldPatronVassalList)
            .Add("newpatronid", newPatron.Id)
            .Add("newpatronname", newPatron.Name)
            .Add("newpatronvassallist", newPatron.VassalIds);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }



    public async Task<AcgmResponse> ChangePasswordAsync(Uri endpoint, string characterName, string password, int characterId, string newPassword, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgChangePassword)
            .Add("charid", characterId)
            .Add("newpw", newPassword)
            .Add("confirmpw", newPassword);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AcgmResponse> BackupDatabaseAsync(Uri endpoint, string characterName, string password, int loginCharacterId, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgBackupDb)
            .Add("charid", loginCharacterId);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }


    public async Task<AcgmResponse> ChangeServerSetupAsync(Uri endpoint, string characterName, string password, ServerSetupOptions options, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgChangeServerSetup)
            .Add("guildname", options.GuildName)
            .Add("use_rescue_squad", options.UseRescueSquad ? "1" : "0")
            .Add("rescue_squad_name", options.RescueSquadName)
            .Add("resetpw", options.ResetExistingDefaultPasswordUsers ? "1" : "0");

        // VB6 only sends defaultpw when the administrator entered a new value.
        // Preserve that behavior so opening Server Setup cannot accidentally
        // replace the default password with a blank value.
        if (!string.IsNullOrWhiteSpace(options.DefaultPassword))
            post.Add("defaultpw", options.DefaultPassword);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AcgmResponse> ResetCharacterPasswordAsync(Uri endpoint, string characterName, string password, int loginCharacterId, int resetCharacterId, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgResetPassword)
            .Add("charid", loginCharacterId)
            .Add("resetid", resetCharacterId);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AcgmResponse> ChangeSecurityLevelAsync(Uri endpoint, string characterName, string password, int loginCharacterId, int changeCharacterId, int newLevel, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgChangeSecLevel)
            .Add("charid", loginCharacterId)
            .Add("changeid", changeCharacterId)
            .Add("newlevel", newLevel);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AcgmResponse> GetRescueSquadAsync(Uri endpoint, string characterName, string password, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgGetRescueSquad);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AcgmResponse> GetPortalListAsync(Uri endpoint, string characterName, string password, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgGetPortalList);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AcgmResponse> GetTradeSkillListAsync(Uri endpoint, string characterName, string password, CancellationToken cancellationToken = default)
    {
        var post = NewPreparedRequest(characterName, password)
            .Add("msgid", AcgmConstants.MsgGetTradeSkillList);

        return await PostAsync(endpoint, post, cancellationToken).ConfigureAwait(false);
    }


    private static string SerializeAwardsForLegacyPost(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var awards = value
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(award => award.Length > 0)
            .ToArray();

        return awards.Length == 0 ? string.Empty : string.Join("!;", awards) + "!;";
    }

    private static void AddChangedText(LegacyPostEncoder post, string key, string oldValue, string newValue)
    {
        if (!string.Equals((oldValue ?? string.Empty).Trim(), (newValue ?? string.Empty).Trim(), StringComparison.Ordinal))
            post.Add(key, newValue ?? string.Empty);
    }

    private static void AddChangedChoice(LegacyPostEncoder post, string key, string oldValue, string newValue)
    {
        AddChangedText(post, key, oldValue, newValue);
    }

    private static void AddChangedBool(LegacyPostEncoder post, string key, bool oldValue, bool newValue)
    {
        if (oldValue != newValue)
            post.Add(key, newValue ? "1" : "0");
    }

    public static LegacyPostEncoder NewPreparedRequest(string characterName, string password)
    {
        return new LegacyPostEncoder()
            .Add("charname", characterName)
            .Add("password", password);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _client.Dispose();
        _disposed = true;
    }
}
