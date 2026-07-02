using ACGM.ModernClient.Models;

namespace ACGM.ModernClient.Protocol;

public sealed record LegacyChoice(string Value, string Text)
{
    public override string ToString() => Text;
}

public static class LegacyCharacterParser
{
    public static readonly IReadOnlyList<LegacyChoice> RaceChoices = new[]
    {
        new LegacyChoice("0", "Unknown"),
        new LegacyChoice("1", "Aluvian"),
        new LegacyChoice("2", "Gharu'ndim"),
        new LegacyChoice("3", "Sho")
    };

    public static readonly IReadOnlyList<LegacyChoice> SexChoices = new[]
    {
        new LegacyChoice("0", "Unknown"),
        new LegacyChoice("1", "Male"),
        new LegacyChoice("2", "Female")
    };

    public static CharacterDetails Parse(string payload)
    {
        var fields = payload.Split('|').Select(LegacyTreeParser.Decode).ToArray();
        var details = new CharacterDetails { RawFieldCount = fields.Length };

        if (fields.Length == 0)
            return details;

        // Legacy VB6 frmMain.GetCharInfo response mapping, 0-based array after Split("|").
        // See frmMain.frm DisplayCharInfo-era logic: strData(1)=name, strData(2)=level,
        // strData(3)=rank, strData(8)=class, strData(9)=race, strData(10)=sex, etc.
        details.Id = ParseInt(Get(fields, 0));
        details.Name = Get(fields, 1);
        details.Level = Get(fields, 2);
        details.Rank = Get(fields, 3);
        details.FollowerCount = Get(fields, 6);
        details.ClassName = Get(fields, 8);
        details.Race = Get(fields, 9);
        details.Sex = Get(fields, 10);
        details.IsMule = IsTrue(Get(fields, 11));
        details.MuleFor = Get(fields, 12);
        details.LifestonedAt = Get(fields, 13);
        details.TiedTo = Get(fields, 14);
        details.Bio = Get(fields, 15);
        details.Skills = Get(fields, 16);
        details.RealName = Get(fields, 17);
        details.CityState = Get(fields, 18);
        details.MiscRealLife = Get(fields, 19);
        details.Email = Get(fields, 20);
        details.Icq = Get(fields, 21);
        details.LastModified = Get(fields, 23);
        details.Patron = Get(fields, 24);
        details.PathToMonarch = Get(fields, 25);
        details.IsPk = IsTrue(Get(fields, 26));
        details.IsRescueMember = IsTrue(Get(fields, 27));
        details.HideInfoOnWeb = IsTrue(Get(fields, 28));
        details.CanSummon = IsTrue(Get(fields, 29));
        details.MainCharacterName = Get(fields, 30);
        details.LastModifiedByCharacterId = ParseInt(Get(fields, 31));
        details.LastModifiedBy = Get(fields, 31);
        details.Awards = ParseAwards(Get(fields, fields.Length - 1));
        return details;
    }

    public static IReadOnlyList<LegacyChoice> GetRankChoices(string raceValue, string sexValue)
    {
        var race = ParseInt(raceValue);
        var sex = ParseInt(sexValue);

        if (race <= 0)
            race = 1; // VB6 defaults unknown race to Aluvian.
        if (sex <= 0)
            sex = 1; // VB6 defaults unknown sex to Male.

        var ranks = (sex, race) switch
        {
            (2, 1) => new[] { "Yeoman", "Baronet(2)", "Baroness", "Reeve", "Thane", "Ealdor", "Duchess", "Aetheling", "Queen", "High Queen" },
            (2, 2) => new[] { "Sayyida", "Shayka", "Maulana", "Mu'allima", "Naqiba", "Qadiya", "Mushira", "Amira", "Malika", "Sultana" },
            (2, 3) => new[] { "Jinin", "Jo-chueh", "Nan-chueh", "Shi-chueh", "Ta-chueh", "Kun-chueh", "Kou", "Taikou", "Jo-ou", "Koutei" },
            (_, 2) => new[] { "Sayyid", "Shayk", "Maulan", "Mu'allim", "Naqib", "Qadi", "Mushir", "Amir", "Malik", "Sultan" },
            (_, 3) => new[] { "Jinin", "Jo-chueh", "Nan-chueh", "Shi-chueh", "Ta-chueh", "Kun-chueh", "Kou", "Taikou", "Ou", "Koutei" },
            _ => new[] { "Yeoman", "Baronet(2)", "Baron", "Reeve", "Thane", "Ealdor", "Duke", "Aetheling", "King", "High King" }
        };

        var result = new List<LegacyChoice> { new("0", "Unknown") };
        for (var i = 0; i < ranks.Length; i++)
        {
            var rankNumber = i + 1;
            var label = ranks[i].Contains('(') ? ranks[i] : $"{ranks[i]} ({rankNumber})";
            result.Add(new LegacyChoice(rankNumber.ToString(), label));
        }
        return result;
    }

    public static string ParseAwards(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        // Character-detail awards live in the final field. Legacy entries can be
        // separated by !; (or may arrive as a single award without a delimiter).
        // Display one award per line while preserving legacy wire compatibility.
        var awards = LegacyTreeParser.Decode(value)
            .Split(new[] { "!;" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(a => a.Trim())
            .Where(a => a.Length > 0);

        return string.Join(Environment.NewLine, awards);
    }

    private static string Get(string[] fields, int index) => index >= 0 && index < fields.Length ? fields[index].Trim() : string.Empty;
    private static int ParseInt(string value) => int.TryParse(value.Trim(), out var result) ? result : 0;
    private static bool IsTrue(string value) => value.Trim() == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase);
}
