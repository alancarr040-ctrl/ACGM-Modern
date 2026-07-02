using ACGM.ModernClient.Configuration;
using ACGM.ModernClient.Forms;

namespace ACGM.ModernClient;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.ThreadException += (_, e) =>
            MessageBox.Show(e.Exception.ToString(), "ACGM Modern Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            MessageBox.Show(e.ExceptionObject?.ToString() ?? "Unknown fatal error", "ACGM Modern Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        var settings = AcgmSettings.Load();
        using var login = new PlayerLoginForm(settings);

        if (login.ShowDialog() != DialogResult.OK)
            return;

        var endpoint = login.ServerEndpoint;
        var characterName = login.CharacterName;
        var password = login.Password;

        Application.Run(new MainForm(endpoint, characterName, password));
    }
}
