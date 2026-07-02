namespace ACGM.ModernClient.Models;

public enum SkillTrainingLevel
{
    Unusable = 0,
    Untrained = 1,
    Trained = 2,
    Specialized = 3
}

public sealed class SkillInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SkillTrainingLevel StartLevel { get; set; }
    public SkillTrainingLevel TrainingLevel { get; set; }
    public int Value { get; set; }

    public bool HasValue => Value > 0;

    public int LegacyOrder => int.TryParse(Id, out var parsed) ? parsed : int.MaxValue;
}
