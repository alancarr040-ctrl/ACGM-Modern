namespace ACGM.ModernClient.Logging;

public static class AcgmLogger
{
    public static void TryWrite(string message)
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "acgm-modern.log");
            File.AppendAllText(path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}{Environment.NewLine}");
        }
        catch
        {
            // Logging must never break the legacy-compatible client workflow.
        }
    }
}
