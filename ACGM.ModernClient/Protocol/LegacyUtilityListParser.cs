using ACGM.ModernClient.Models;

namespace ACGM.ModernClient.Protocol;

public static class LegacyUtilityListParser
{
    public static IReadOnlyList<RescueSquadEntry> ParseRescueSquad(string payload)
    {
        var entries = new List<RescueSquadEntry>();
        foreach (var fields in SplitRows(payload))
        {
            entries.Add(new RescueSquadEntry
            {
                Name = Get(fields, 1),
                Level = Get(fields, 2),
                Lifestone = Get(fields, 3),
                TiedTo = Get(fields, 4),
                CanSummon = IsTrue(Get(fields, 5)) ? "Yes" : "No",
                MainCharacterName = Get(fields, 6),
                EmailAddress = Get(fields, 7),
                IcqNumber = Get(fields, 8)
            });
        }
        return entries;
    }

    public static IReadOnlyList<PortalListEntry> ParsePortalList(string payload)
    {
        var entries = new List<PortalListEntry>();
        foreach (var fields in SplitRows(payload))
        {
            entries.Add(new PortalListEntry
            {
                Name = Get(fields, 1),
                Level = Get(fields, 2),
                TiedTo = Get(fields, 3),
                Lifestone = Get(fields, 4)
            });
        }
        return entries;
    }

    public static IReadOnlyList<TradeSkillEntry> ParseTradeSkills(string payload)
    {
        var entries = new List<TradeSkillEntry>();
        foreach (var fields in SplitRows(payload))
        {
            entries.Add(new TradeSkillEntry
            {
                Name = Get(fields, 1),
                Level = Get(fields, 2),
                Alchemy = Get(fields, 3),
                Cooking = Get(fields, 4),
                Fletching = Get(fields, 5)
            });
        }
        return entries;
    }

    private static IEnumerable<string[]> SplitRows(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
            yield break;

        foreach (var row in payload.Split("!;", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            yield return row.Split('|').Select(LegacyTreeParser.Decode).ToArray();
    }

    private static string Get(string[] fields, int index) => index >= 0 && index < fields.Length ? fields[index].Trim() : string.Empty;
    private static bool IsTrue(string value) => value.Trim() == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase) || value.Equals("yes", StringComparison.OrdinalIgnoreCase);
}
