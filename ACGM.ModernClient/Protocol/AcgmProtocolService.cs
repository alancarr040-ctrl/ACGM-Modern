using ACGM.ModernClient.Models;
using ACGM.ModernClient.Logging;

namespace ACGM.ModernClient.Protocol;

public sealed class AcgmProtocolService
{
    private readonly Uri _endpoint;
    private readonly string _loginCharacter;
    private readonly string _password;

    public AcgmProtocolService(Uri endpoint, string loginCharacter, string password)
    {
        _endpoint = endpoint;
        _loginCharacter = loginCharacter;
        _password = password;
    }

    public async Task<(int SecurityLevel, string Payload, int Bytes)> LoginAsync(CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.LoginAsync(_endpoint, _loginCharacter, _password, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");

        // Legacy DisplayLoginInfo splits the login payload on | and reads
        // txtRetData(3) as iCurSecurityLevel. 1 = Normal User, 3 = Administrator.
        var fields = response.Payload.Split('|');
        var securityLevel = fields.Length > 3 && int.TryParse(fields[3].Trim(), out var parsed) ? parsed : 1;
        return (securityLevel, response.Payload, response.Payload.Length);
    }

    public async Task<(List<CharacterRecord> Records, int Bytes)> GetTreeAsync(CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.GetTreeAsync(_endpoint, _loginCharacter, _password, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        var records = LegacyTreeParser.Parse(response.Payload);
        AcgmDiagnostics.WriteTreePayload(response.Payload, response.RawBody, records);
        AcgmDiagnostics.WriteTreeIconDiagnostics(records);
        return (records, response.Payload.Length);
    }

    public async Task<(CharacterDetails Details, int Bytes)> GetCharacterInfoAsync(int characterId, CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.GetCharacterInfoAsync(_endpoint, _loginCharacter, _password, characterId.ToString(), cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        var details = LegacyCharacterParser.Parse(response.Payload);
        AcgmDiagnostics.WriteCharacterDetailPayload(characterId, response.Payload, details);
        return (details, response.Payload.Length);
    }

    public async Task<int> SaveCharacterInfoAsync(int loginCharacterId, int updateCharacterId, CharacterDetails original, CharacterDetails updated, CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.UpdateCharacterInfoAsync(_endpoint, _loginCharacter, _password, loginCharacterId, updateCharacterId, original, updated, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        return response.Payload.Length;
    }


    public async Task<(string Payload, int Bytes)> BackupDatabaseAsync(int loginCharacterId, CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.BackupDatabaseAsync(_endpoint, _loginCharacter, _password, loginCharacterId, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        return (response.Payload, response.Payload.Length);
    }


    public async Task<int> ChangeServerSetupAsync(ServerSetupOptions options, CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.ChangeServerSetupAsync(_endpoint, _loginCharacter, _password, options, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        return response.Payload.Length;
    }

    public async Task<int> ResetCharacterPasswordAsync(int loginCharacterId, int resetCharacterId, CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.ResetCharacterPasswordAsync(_endpoint, _loginCharacter, _password, loginCharacterId, resetCharacterId, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        return response.Payload.Length;
    }

    public async Task<int> ChangeSecurityLevelAsync(int loginCharacterId, int changeCharacterId, int newLevel, CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.ChangeSecurityLevelAsync(_endpoint, _loginCharacter, _password, loginCharacterId, changeCharacterId, newLevel, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        return response.Payload.Length;
    }

    public async Task<(IReadOnlyList<RescueSquadEntry> Entries, int Bytes)> GetRescueSquadAsync(CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.GetRescueSquadAsync(_endpoint, _loginCharacter, _password, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        return (LegacyUtilityListParser.ParseRescueSquad(response.Payload), response.Payload.Length);
    }

    public async Task<(IReadOnlyList<PortalListEntry> Entries, int Bytes)> GetPortalListAsync(CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.GetPortalListAsync(_endpoint, _loginCharacter, _password, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        return (LegacyUtilityListParser.ParsePortalList(response.Payload), response.Payload.Length);
    }

    public async Task<(IReadOnlyList<TradeSkillEntry> Entries, int Bytes)> GetTradeSkillListAsync(CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.GetTradeSkillListAsync(_endpoint, _loginCharacter, _password, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        return (LegacyUtilityListParser.ParseTradeSkills(response.Payload), response.Payload.Length);
    }

    public async Task<int> AddVassalAsync(int loginCharacterId, CharacterRecord targetPatron, string vassalName, CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.AddVassalAsync(_endpoint, _loginCharacter, _password, loginCharacterId, targetPatron, vassalName, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        return response.Payload.Length;
    }

    public async Task<int> RemoveVassalAsync(int loginCharacterId, CharacterRecord targetPatron, CharacterRecord vassal, CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.RemoveVassalAsync(_endpoint, _loginCharacter, _password, loginCharacterId, targetPatron, vassal, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        return response.Payload.Length;
    }

    public async Task<int> ChangePatronAsync(int loginCharacterId, CharacterRecord movingCharacter, CharacterRecord? oldPatron, CharacterRecord newPatron, CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.ChangePatronAsync(_endpoint, _loginCharacter, _password, loginCharacterId, movingCharacter, oldPatron, newPatron, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        return response.Payload.Length;
    }

    public async Task<int> ChangePasswordAsync(int characterId, string newPassword, CancellationToken cancellationToken = default)
    {
        using var client = new AcgmHttpClient();
        var response = await client.ChangePasswordAsync(_endpoint, _loginCharacter, _password, characterId, newPassword, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
            throw new InvalidOperationException($"Server returned legacy result {response.ResultCode}.");
        return response.Payload.Length;
    }

}
