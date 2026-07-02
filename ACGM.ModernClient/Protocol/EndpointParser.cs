namespace ACGM.ModernClient.Protocol;

public static class EndpointParser
{
    public static Uri ParseServerInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Server endpoint is required.", nameof(input));

        input = input.Trim();

        if (!input.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !input.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            input = "https://" + input;
        }

        if (!Uri.TryCreate(input, UriKind.Absolute, out var uri))
            throw new FormatException("Server endpoint is not a valid URL.");

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            throw new FormatException("Server endpoint must use http:// or https://.");

        return uri;
    }
}
