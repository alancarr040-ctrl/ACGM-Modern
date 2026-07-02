namespace ACGM.ModernClient.Models;

public sealed class CharacterDetails
{
    public int Id { get; set; }
    public int RawFieldCount { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public string ComputedRank { get; set; } = string.Empty;
    public string FollowerCount { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public string Sex { get; set; } = string.Empty;
    public bool IsMule { get; set; }
    public string MuleFor { get; set; } = string.Empty;
    public string MainCharacterName { get; set; } = string.Empty;
    public string LifestonedAt { get; set; } = string.Empty;
    public string TiedTo { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string CityState { get; set; } = string.Empty;
    public string MiscRealLife { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Icq { get; set; } = string.Empty;
    public string Patron { get; set; } = string.Empty;
    public string PathToMonarch { get; set; } = string.Empty;
    public bool IsPk { get; set; }
    public bool IsRescueMember { get; set; }
    public bool HideInfoOnWeb { get; set; }
    public bool CanSummon { get; set; }
    public string LastModified { get; set; } = string.Empty;
    public int LastModifiedByCharacterId { get; set; }
    public string LastModifiedBy { get; set; } = string.Empty;
    public string Awards { get; set; } = string.Empty;
}
