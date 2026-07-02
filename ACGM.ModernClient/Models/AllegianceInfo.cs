namespace ACGM.ModernClient.Models;

public sealed class AllegianceInfo
{
    public int CharacterId { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public string Patron { get; set; } = string.Empty;
    public string Monarch { get; set; } = string.Empty;
    public string PathToMonarch { get; set; } = string.Empty;
    public int DirectVassalCount { get; set; }
    public int TotalKnownVassalCount { get; set; }
    public IReadOnlyList<CharacterRecord> DirectVassals { get; set; } = Array.Empty<CharacterRecord>();
    public IReadOnlyList<CharacterRecord> KnownDescendants { get; set; } = Array.Empty<CharacterRecord>();
    public string Notes { get; set; } = string.Empty;
    public string SourceSummary { get; set; } = string.Empty;
}
