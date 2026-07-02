using System.Text;

namespace ACGM.ModernClient.Configuration;

public sealed class AcgmSettings
{
    public string Version { get; set; } = "0.6";
    public int TimeoutSeconds { get; set; } = 30;
    public int UsePics { get; set; } = 1;
    public string LastServerUsed { get; set; } = "";
    public string LastPlayerUsed { get; set; } = "";
    public int MainSplitterDistance { get; set; } = 0;
    public int MainWindowLeft { get; set; } = -1;
    public int MainWindowTop { get; set; } = -1;
    public int MainWindowWidth { get; set; } = 980;
    public int MainWindowHeight { get; set; } = 720;
    public string LastSelectedCharacter { get; set; } = "";
    public int SelectedDetailTabIndex { get; set; } = 0;
    public bool CharacterSaveDiagnostics { get; set; } = false;
    public Dictionary<string, List<string>> Servers { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static string DefaultPath => Path.Combine(AppContext.BaseDirectory, "acgm.ini");

    public static AcgmSettings Load(string? path = null)
    {
        path ??= DefaultPath;
        var settings = new AcgmSettings();

        if (!File.Exists(path))
        {
            settings.AddServer("www.yourserver.com/cgi-bin/acgm/server.cgi");
            return settings;
        }

        string? currentServer = null;
        string currentSection = "";
        foreach (var rawLine in File.ReadLines(path, Encoding.UTF8))
        {
            var line = rawLine.Trim();
            if (line.Length == 0) continue;

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                currentSection = line[1..^1].Trim();
                if (string.Equals(currentSection, "Diagnostics", StringComparison.OrdinalIgnoreCase))
                {
                    currentServer = null;
                }
                else
                {
                    currentServer = currentSection;
                    settings.AddServer(currentServer);
                }
                continue;
            }

            var equals = line.IndexOf('=');
            if (equals < 0) continue;

            var key = line[..equals].Trim();
            var value = line[(equals + 1)..].Trim();

            if (string.Equals(currentSection, "Diagnostics", StringComparison.OrdinalIgnoreCase))
            {
                switch (key.ToUpperInvariant())
                {
                    case "CHARACTERSAVE":
                    case "CHARACTER_SAVE":
                    case "CHARACTER_SAVE_DIAGNOSTICS":
                        settings.CharacterSaveDiagnostics = IsTruthy(value);
                        break;
                }
                continue;
            }

            switch (key.ToUpperInvariant())
            {
                case "VERSION":
                    settings.Version = value;
                    break;
                case "TIMEOUT":
                    if (int.TryParse(value, out var timeout) && timeout > 0)
                        settings.TimeoutSeconds = timeout;
                    break;
                case "USEPICS":
                    if (int.TryParse(value, out var usePics))
                        settings.UsePics = usePics;
                    break;
                case "LASTSERVERUSED":
                    settings.LastServerUsed = value;
                    settings.AddServer(value);
                    break;
                case "LASTPLAYERUSED":
                    settings.LastPlayerUsed = value;
                    break;
                case "MAINSPLITTERDISTANCE":
                    if (int.TryParse(value, out var splitterDistance) && splitterDistance > 0)
                        settings.MainSplitterDistance = splitterDistance;
                    break;
                case "MAINWINDOWLEFT":
                    if (int.TryParse(value, out var left))
                        settings.MainWindowLeft = left;
                    break;
                case "MAINWINDOWTOP":
                    if (int.TryParse(value, out var top))
                        settings.MainWindowTop = top;
                    break;
                case "MAINWINDOWWIDTH":
                    if (int.TryParse(value, out var width) && width >= 640)
                        settings.MainWindowWidth = width;
                    break;
                case "MAINWINDOWHEIGHT":
                    if (int.TryParse(value, out var height) && height >= 480)
                        settings.MainWindowHeight = height;
                    break;
                case "LASTSELECTEDCHARACTER":
                    settings.LastSelectedCharacter = value;
                    break;
                case "SELECTEDDETAILTABINDEX":
                    if (int.TryParse(value, out var tabIndex) && tabIndex >= 0)
                        settings.SelectedDetailTabIndex = tabIndex;
                    break;
                case "PLAYER":
                    if (!string.IsNullOrWhiteSpace(currentServer))
                        settings.AddPlayer(currentServer, value);
                    break;
            }
        }

        if (settings.Servers.Count == 0)
            settings.AddServer("www.yourserver.com/cgi-bin/acgm/server.cgi");

        return settings;
    }

    public void Save(string? path = null)
    {
        path ??= DefaultPath;
        using var writer = new StreamWriter(path, false, Encoding.UTF8);
        writer.WriteLine($"Version = {Version}");
        writer.WriteLine($"Timeout = {TimeoutSeconds}");
        writer.WriteLine($"UsePics = {UsePics}");
        writer.WriteLine($"LastServerUsed = {LastServerUsed}");
        writer.WriteLine($"LastPlayerUsed = {LastPlayerUsed}");
        writer.WriteLine($"MainSplitterDistance = {MainSplitterDistance}");
        writer.WriteLine($"MainWindowLeft = {MainWindowLeft}");
        writer.WriteLine($"MainWindowTop = {MainWindowTop}");
        writer.WriteLine($"MainWindowWidth = {MainWindowWidth}");
        writer.WriteLine($"MainWindowHeight = {MainWindowHeight}");
        writer.WriteLine($"LastSelectedCharacter = {LastSelectedCharacter}");
        writer.WriteLine($"SelectedDetailTabIndex = {SelectedDetailTabIndex}");
        writer.WriteLine();
        writer.WriteLine("[Diagnostics]");
        writer.WriteLine($"CharacterSave = {(CharacterSaveDiagnostics ? 1 : 0)}");

        foreach (var pair in Servers.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
        {
            writer.WriteLine($"[{pair.Key}]");
            foreach (var player in pair.Value.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
                writer.WriteLine($"Player = {player}");
        }
    }

    public void AddServer(string server)
    {
        server = NormalizeServerForStorage(server);
        if (server.Length == 0) return;
        if (!Servers.ContainsKey(server))
            Servers[server] = new List<string>();
    }

    public void AddPlayer(string server, string player)
    {
        server = NormalizeServerForStorage(server);
        player = player.Trim();
        if (server.Length == 0 || player.Length == 0) return;
        AddServer(server);
        if (!Servers[server].Contains(player, StringComparer.OrdinalIgnoreCase))
            Servers[server].Add(player);
    }

    private static bool IsTruthy(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        value = value.Trim();
        return value.Equals("1", StringComparison.OrdinalIgnoreCase)
            || value.Equals("true", StringComparison.OrdinalIgnoreCase)
            || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
            || value.Equals("on", StringComparison.OrdinalIgnoreCase);
    }

    public static string NormalizeServerForStorage(string value)
    {
        value = value.Trim();
        if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            value = value[7..];
        if (value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            value = value[8..];
        return value.TrimEnd('/');
    }

    public static Uri NormalizeServerForConnection(string value)
    {
        value = value.Trim();
        if (!value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            value = "https://" + value;

        return Protocol.EndpointParser.ParseServerInput(value);
    }
}
