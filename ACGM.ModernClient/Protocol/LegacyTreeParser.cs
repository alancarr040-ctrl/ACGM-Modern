using ACGM.ModernClient.Models;

namespace ACGM.ModernClient.Protocol;

public static class LegacyTreeParser
{
    public static List<CharacterRecord> Parse(string payload)
    {
        var records = new List<CharacterRecord>();
        if (string.IsNullOrWhiteSpace(payload))
            return records;

        foreach (var rawRecord in payload.Split("!;", StringSplitOptions.RemoveEmptyEntries))
        {
            var fields = rawRecord.Trim().Split('|');
            if (fields.Length < 9)
                continue;

            records.Add(new CharacterRecord
            {
                Id = ParseInt(fields[0]),
                Name = Decode(fields[1]),
                Level = ParseInt(fields[2]),
                Rank = ParseInt(fields[3]),
                PatronId = ParseInt(fields[4]),
                VassalIds = fields[5].Trim(),

                // Legacy VB6 mapping from Characters.Add:
                //   strCharInfo(6) => iIsMule
                //   strCharInfo(7) => iIsPK
                //   strCharInfo(8) => iIsRescueSquadMember
                // The tree payload has only 9 fields and does not include race.
                RawTreeFlag6 = fields[6].Trim(),
                RawTreeFlag7 = fields[7].Trim(),
                RawTreeFlag8 = fields[8].Trim(),
                IsMule = ParseInt(fields[6]) == 1,
                IsPk = ParseInt(fields[7]) == 1,
                IsRescueSquad = ParseInt(fields[8]) == 1
            });
        }

        var byId = records.ToDictionary(r => r.Id);
        foreach (var record in records)
        {
            if (byId.TryGetValue(record.PatronId, out var patron))
                record.PatronName = patron.Name;
        }

        return records;
    }

    private static int ParseInt(string value) => int.TryParse(value.Trim(), out var result) ? result : 0;

    internal static string Decode(string value)
    {
        return value
            .Replace("(pipe)", "|")
            .Replace("(end)", "!;")
            .Replace("(amp)", "&")
            .Trim();
    }
}
