namespace ACGM.ModernClient.Models;

public sealed class CharacterRecord
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PatronId { get; set; }
    public string PatronName { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Rank { get; set; }
    public int ComputedRank { get; set; }
    public int Race { get; set; }
    public string VassalIds { get; set; } = string.Empty;
    public bool IsMule { get; set; }
    public bool IsPk { get; set; }
    public bool IsRescueSquad { get; set; }
    public string RawTreeFlag6 { get; set; } = string.Empty;
    public string RawTreeFlag7 { get; set; } = string.Empty;
    public string RawTreeFlag8 { get; set; } = string.Empty;
    public string DisplayText => $"{Name} - Level {Level}";
}
