using ACGM.ModernClient.Models;

namespace ACGM.ModernClient.Protocol;

public static class LegacySkillParser
{
    public static readonly IReadOnlyList<SkillInfo> SkillDefinitions = new[]
    {
        Def("1", "Alchemy", 0),
        Def("2", "Appraise Armor", 1),
        Def("3", "Appraise Item", 1),
        Def("4", "Appraise Magic Item", 1),
        Def("5", "Appraise Weapon", 1),
        Def("6", "Arcane Lore", 1),
        Def("7", "Assess Creature", 1),
        Def("8", "Assess Person", 1),
        Def("9", "Axe", 1),
        Def("10", "Bow", 1),
        Def("11", "Cooking", 0),
        Def("12", "Creature Enchantment", 0),
        Def("13", "Crossbow", 1),
        Def("14", "Dagger", 1),
        Def("15", "Deception", 1),
        Def("16", "Fletching", 0),
        Def("17", "Healing", 0),
        Def("18", "Item Enchantment", 0),
        Def("19", "Jump", 2),
        Def("20", "Leadership", 1),
        Def("21", "Life Magic", 0),
        Def("22", "Lockpick", 0),
        Def("23", "Loyalty", 2),
        Def("24", "Mace", 1),
        Def("25", "Magic Defense", 2),
        Def("26", "Mana Conversion", 0),
        Def("27", "Melee Defense", 1),
        Def("28", "Missle Defense", 1),
        Def("29", "Run", 2),
        Def("30", "Spear", 1),
        Def("31", "Staff", 1),
        Def("32", "Sword", 1),
        Def("33", "Thrown Weapons", 1),
        Def("34", "Unarmed Combat", 1),
        Def("35", "War Magic", 0)
    };

    public static List<SkillInfo> Parse(string? rawSkills)
    {
        var skills = SkillDefinitions.Select(Clone).ToList();
        var byId = skills.ToDictionary(s => s.Id, StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(rawSkills))
            return skills;

        foreach (var chunk in rawSkills.Split('!', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = chunk.Split(',', StringSplitOptions.None);
            if (parts.Length < 2)
                continue;

            var id = parts[0].Trim();
            if (!byId.TryGetValue(id, out var skill))
                continue;

            skill.TrainingLevel = ToTrainingLevel(parts[1]);
            if (parts.Length >= 3 && int.TryParse(parts[2].Trim(), out var value))
                skill.Value = value;
        }

        return skills;
    }

    public static string Serialize(IEnumerable<SkillInfo> skills)
    {
        return string.Join("!", skills
            .OrderBy(s => int.TryParse(s.Id, out var id) ? id : int.MaxValue)
            .Select(s => $"{s.Id},{(int)s.TrainingLevel},{s.Value}"));
    }

    public static string TrainingLabel(SkillTrainingLevel level) => level switch
    {
        SkillTrainingLevel.Specialized => "Specialized",
        SkillTrainingLevel.Trained => "Trained",
        SkillTrainingLevel.Untrained => "Untrained",
        SkillTrainingLevel.Unusable => "Unusable",
        _ => "Unknown"
    };

    private static SkillInfo Def(string id, string name, int startLevel) => new()
    {
        Id = id,
        Name = name,
        StartLevel = ToTrainingLevel(startLevel.ToString()),
        TrainingLevel = ToTrainingLevel(startLevel.ToString()),
        Value = 0
    };

    private static SkillInfo Clone(SkillInfo source) => new()
    {
        Id = source.Id,
        Name = source.Name,
        StartLevel = source.StartLevel,
        TrainingLevel = source.StartLevel,
        Value = source.Value
    };

    private static SkillTrainingLevel ToTrainingLevel(string value) => value.Trim() switch
    {
        "3" => SkillTrainingLevel.Specialized,
        "2" => SkillTrainingLevel.Trained,
        "1" => SkillTrainingLevel.Untrained,
        _ => SkillTrainingLevel.Unusable
    };
}
