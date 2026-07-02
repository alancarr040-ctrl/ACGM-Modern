namespace ACGM.ModernClient.Models;

public sealed class RescueSquadEntry
{
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Lifestone { get; set; } = string.Empty;
    public string TiedTo { get; set; } = string.Empty;
    public string CanSummon { get; set; } = string.Empty;
    public string MainCharacterName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string IcqNumber { get; set; } = string.Empty;
}

public sealed class PortalListEntry
{
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string TiedTo { get; set; } = string.Empty;
    public string Lifestone { get; set; } = string.Empty;
}

public sealed class TradeSkillEntry
{
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Alchemy { get; set; } = string.Empty;
    public string Cooking { get; set; } = string.Empty;
    public string Fletching { get; set; } = string.Empty;
}
