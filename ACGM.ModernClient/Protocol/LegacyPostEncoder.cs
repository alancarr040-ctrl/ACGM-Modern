using System.Text;

namespace ACGM.ModernClient.Protocol;

public sealed class LegacyPostEncoder
{
    private readonly List<KeyValuePair<string, string>> _fields = new();

    public IReadOnlyList<KeyValuePair<string, string>> Fields => _fields;

    public LegacyPostEncoder Add(string name, object? value)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name is required.", nameof(name));

        _fields.Add(new KeyValuePair<string, string>(name, EscapeLegacyValue(Convert.ToString(value) ?? string.Empty)));
        return this;
    }

    public string BuildBody()
    {
        // The legacy VB6 client did not URL-encode these fields. It performed only
        // three custom substitutions before joining name=value pairs with ampersands.
        var builder = new StringBuilder();
        for (var i = 0; i < _fields.Count; i++)
        {
            if (i > 0) builder.Append('&');
            builder.Append(_fields[i].Key).Append('=').Append(_fields[i].Value);
        }
        return builder.ToString();
    }

    public static string EscapeLegacyValue(string value)
    {
        return value
            .Replace("|", "(pipe)", StringComparison.Ordinal)
            .Replace("!;", "(end)", StringComparison.Ordinal)
            .Replace("&", "(amp)", StringComparison.Ordinal);
    }
}
