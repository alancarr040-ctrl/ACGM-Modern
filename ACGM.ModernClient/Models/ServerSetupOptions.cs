namespace ACGM.ModernClient.Models;

public sealed class ServerSetupOptions
{
    public string GuildName { get; set; } = string.Empty;
    public string DefaultPassword { get; set; } = string.Empty;
    public bool ResetExistingDefaultPasswordUsers { get; set; }
    public bool UseRescueSquad { get; set; }
    public string RescueSquadName { get; set; } = string.Empty;
}
