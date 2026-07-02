namespace ACGM.ModernClient.Protocol;

public sealed class AcgmResponse
{
    public required int ResultCode { get; init; }
    public required string Payload { get; init; }
    public required string RawBody { get; init; }
    public bool IsSuccess => ResultCode >= 800 && ResultCode < 900;

    public static AcgmResponse Parse(string rawBody)
    {
        var startIndex = rawBody.IndexOf(AcgmConstants.DataStartString, StringComparison.Ordinal);
        if (startIndex < 0)
            throw new InvalidOperationException("The response did not contain the legacy <START_HERE> marker.");

        var afterStart = startIndex + AcgmConstants.DataStartString.Length;
        var body = rawBody[afterStart..];

        var separatorIndex = body.IndexOf(';');
        if (separatorIndex <= 0)
            throw new InvalidOperationException("The response did not contain a result code followed by ';'.");

        if (!int.TryParse(body[..separatorIndex].Trim(), out var resultCode))
            throw new InvalidOperationException("The response result code was not numeric.");

        var payload = body[(separatorIndex + 1)..];
        var stopIndex = payload.IndexOf(AcgmConstants.DataStopString, StringComparison.Ordinal);
        if (stopIndex >= 0)
            payload = payload[..stopIndex];

        return new AcgmResponse
        {
            ResultCode = resultCode,
            Payload = payload,
            RawBody = rawBody
        };
    }
}
