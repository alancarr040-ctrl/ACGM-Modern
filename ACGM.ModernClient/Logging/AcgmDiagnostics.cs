using ACGM.ModernClient.Models;
using ACGM.ModernClient.Protocol;
using System.Text;

namespace ACGM.ModernClient.Logging;

public static class AcgmDiagnostics
{
    private static readonly object SyncRoot = new();

    public static string DiagnosticsDirectory
    {
        get
        {
            var path = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(path);
            return path;
        }
    }

    public static string CharacterDiagnosticsPath => Path.Combine(DiagnosticsDirectory, "character-flags-awards-diagnostics.log");
    public static string CharacterSaveDiagnosticsPath => Path.Combine(DiagnosticsDirectory, "character-save-diagnostics.log");
    public static string TreePayloadDiagnosticsPath => Path.Combine(DiagnosticsDirectory, "tree-payload-diagnostics.log");

    public static bool IsCharacterSaveCertificationEnabled => IsDiagnosticsEnabled("CharacterSave")
        || IsEnabled("CHARACTER_SAVE_CERTIFICATION")
        || IsEnabled("ENABLE_CHARACTER_SAVE_DIAGNOSTICS");

    public static void WriteCharacterDetailPayload(int characterId, string payload, CharacterDetails details)
    {
        TryWrite(builder =>
        {
            builder.AppendLine("============================================================");
            builder.AppendLine($"ACGM Character Diagnostics - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            builder.AppendLine($"Character ID Requested: {characterId}");
            builder.AppendLine($"Parsed Name: {details.Name}");
            builder.AppendLine($"Raw Payload Length: {payload.Length}");
            builder.AppendLine();
            builder.AppendLine("Raw character detail payload:");
            builder.AppendLine(payload);
            builder.AppendLine();

            var fields = payload.Split('|').Select(LegacyTreeParser.Decode).ToArray();
            builder.AppendLine($"Raw field count: {fields.Length}");
            for (var i = 0; i < fields.Length; i++)
            {
                builder.AppendLine($"  [{i:00}] = {fields[i]}");
            }

            builder.AppendLine();
            builder.AppendLine("Parsed flag/award fields:");
            builder.AppendLine($"  Field[26] PK raw: '{Get(fields, 26)}' => Parsed IsPk: {details.IsPk}");
            builder.AppendLine($"  Field[27] Rescue raw: '{Get(fields, 27)}' => Parsed IsRescueMember: {details.IsRescueMember}");
            builder.AppendLine($"  Field[11] Mule raw: '{Get(fields, 11)}' => Parsed IsMule: {details.IsMule}");
            builder.AppendLine($"  Final field Awards/Admin raw: '{Get(fields, fields.Length - 1)}' => Parsed Awards: '{details.Awards}'");
            builder.AppendLine($"  ADMIN_ONLY / Awards mapping uses the final character-detail field.");

            if (fields.Length > 33)
            {
                builder.AppendLine();
                builder.AppendLine("Unknown trailing fields after expected Field[32]:");
                for (var i = 33; i < fields.Length; i++)
                    builder.AppendLine($"  [{i:00}] = {fields[i]}");
            }

            builder.AppendLine();
        });
    }


    public static void WriteCharacterSaveDiagnostics(
        CharacterDetails original,
        CharacterDetails updated,
        IReadOnlyList<KeyValuePair<string, string>> postFields,
        string outgoingPostPayload,
        AcgmResponse response)
    {
        if (!IsCharacterSaveCertificationEnabled)
            return;

        TryWrite(CharacterSaveDiagnosticsPath, builder =>
        {
            builder.AppendLine("============================================================");
            builder.AppendLine($"ACGM Character Save Certification Suite - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            builder.AppendLine("Package: 0.11.13 - Legacy Character Save Certification Suite");
            builder.AppendLine("Scope: developer-only MSG_UPDATE_CHAR_INFO certification diagnostics");
            builder.AppendLine();

            builder.AppendLine("Current character values before save:");
            WriteCharacterValueComparison(builder, "Name", original.Name, updated.Name);
            WriteCharacterValueComparison(builder, "Level", original.Level, updated.Level);
            WriteCharacterValueComparison(builder, "Class", original.ClassName, updated.ClassName);
            WriteCharacterValueComparison(builder, "Race", original.Race, updated.Race);
            WriteCharacterValueComparison(builder, "Rank", original.Rank, updated.Rank);
            WriteCharacterValueComparison(builder, "Sex", original.Sex, updated.Sex);
            WriteCharacterValueComparison(builder, "IsMule", original.IsMule, updated.IsMule);
            WriteCharacterValueComparison(builder, "MuleFor", original.MuleFor, updated.MuleFor);
            WriteCharacterValueComparison(builder, "LifestonedAt", original.LifestonedAt, updated.LifestonedAt);
            WriteCharacterValueComparison(builder, "TiedTo", original.TiedTo, updated.TiedTo);
            WriteCharacterValueComparison(builder, "Bio", original.Bio, updated.Bio);
            WriteCharacterValueComparison(builder, "IsPk", original.IsPk, updated.IsPk);
            WriteCharacterValueComparison(builder, "IsRescue", original.IsRescueMember, updated.IsRescueMember);
            WriteCharacterValueComparison(builder, "HideInfoOnWeb", original.HideInfoOnWeb, updated.HideInfoOnWeb);
            WriteCharacterValueComparison(builder, "CanSummon", original.CanSummon, updated.CanSummon);
            WriteCharacterValueComparison(builder, "MainCharacter", original.MainCharacterName, updated.MainCharacterName);
            WriteCharacterValueComparison(builder, "Skills", original.Skills, updated.Skills);
            WriteCharacterValueComparison(builder, "RealName", original.RealName, updated.RealName);
            WriteCharacterValueComparison(builder, "CityState", original.CityState, updated.CityState);
            WriteCharacterValueComparison(builder, "MiscRealLife", original.MiscRealLife, updated.MiscRealLife);
            WriteCharacterValueComparison(builder, "Email", original.Email, updated.Email);
            WriteCharacterValueComparison(builder, "ICQ", original.Icq, updated.Icq);
            WriteCharacterValueComparison(builder, "Awards", original.Awards, updated.Awards);
            builder.AppendLine();

            builder.AppendLine("Expected VB6 MSG_UPDATE_CHAR_INFO POST field order:");
            var expected = new[]
            {
                "charname", "password", "charid", "updateid", "msgid",
                "NAME", "LEVEL", "CLASS", "RACE", "RANK", "SEX", "ISMULE",
                "MULEFOR", "LSAT", "TIEDTO", "BIO", "ISPK", "ISRESCUE",
                "ISPRIVATE", "CAN_SUMMON", "MAIN_CHAR", "SKILLS", "REALNAME",
                "CITYSTATE", "MISCINFO", "EMAIL", "ICQ", "ADMIN_ONLY"
            };
            for (var i = 0; i < expected.Length; i++)
                builder.AppendLine($"  [{i:00}] {expected[i]}");
            builder.AppendLine();

            builder.AppendLine("Current serializer field order and values:");
            for (var i = 0; i < postFields.Count; i++)
                builder.AppendLine($"  [{i:00}] {postFields[i].Key} = {postFields[i].Value}");
            builder.AppendLine();

            builder.AppendLine("Field-by-field comparison against expected order:");
            for (var i = 0; i < postFields.Count; i++)
            {
                var expectedName = i < expected.Length ? expected[i] : "<extra>";
                var actualName = postFields[i].Key;
                var status = string.Equals(expectedName, actualName, StringComparison.OrdinalIgnoreCase) ? "OK" : "CHECK";
                builder.AppendLine($"  [{i:00}] expected={expectedName}; actual={actualName}; {status}");
            }
            builder.AppendLine();

            builder.AppendLine("Final serialized legacy payload:");
            builder.AppendLine(outgoingPostPayload);
            builder.AppendLine();

            builder.AppendLine("Outgoing POST payload:");
            builder.AppendLine(outgoingPostPayload);
            builder.AppendLine();

            builder.AppendLine("Server response:");
            builder.AppendLine($"  ResultCode: {response.ResultCode}");
            builder.AppendLine($"  IsSuccess: {response.IsSuccess}");
            builder.AppendLine($"  Payload Length: {response.Payload.Length}");
            builder.AppendLine(response.RawBody);
            builder.AppendLine();

            builder.AppendLine("Reloaded Character Values:");
            builder.AppendLine("  Not captured automatically. Reload the character after save and compare against this log when certifying manually.");
            builder.AppendLine();
        });
    }

    public static void WriteTreePayload(string payload, string rawBody, IReadOnlyList<CharacterRecord> records)
    {
        TryWrite(TreePayloadDiagnosticsPath, builder =>
        {
            builder.AppendLine("============================================================");
            builder.AppendLine($"ACGM Tree Payload Diagnostics - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            builder.AppendLine($"Payload Length: {payload.Length}");
            builder.AppendLine($"Raw Body Length: {rawBody.Length}");
            builder.AppendLine($"Record Count By Legacy !; Split: {payload.Split("!;", StringSplitOptions.RemoveEmptyEntries).Length}");
            builder.AppendLine($"Parsed Character Records: {records.Count}");
            builder.AppendLine();
            builder.AppendLine("Legacy tree field mapping:");
            builder.AppendLine("  Field[6] => IsMule");
            builder.AppendLine("  Field[7] => IsPK");
            builder.AppendLine("  Field[8] => IsRescueSquadMember");
            builder.AppendLine();
            builder.AppendLine("Raw parsed payload:");
            builder.AppendLine(payload);
            builder.AppendLine();
            builder.AppendLine("Raw HTTP body after decoding:");
            builder.AppendLine(rawBody);
            builder.AppendLine();
        });
    }

    public static void WriteTreeIconDiagnostics(IEnumerable<CharacterRecord> records)
    {
        TryWrite(builder =>
        {
            builder.AppendLine("============================================================");
            builder.AppendLine($"ACGM Tree Icon Diagnostics - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            builder.AppendLine("Icon priority: Monarch -> Mule -> Rescue Squad -> PK -> Normal");
            foreach (var record in records)
            {
                var icon = GetTreeIconReason(record, out var reason);
                builder.AppendLine($"ID={record.Id}; Name='{record.Name}'; Level={record.Level}; RawFlags='{record.RawTreeFlag6}|{record.RawTreeFlag7}|{record.RawTreeFlag8}'; IsMule={record.IsMule}; IsRescueSquad={record.IsRescueSquad}; IsPk={record.IsPk}; Icon='{icon}'; Reason='{reason}'");
            }
            builder.AppendLine();
        });
    }

    public static string GetTreeIconReason(CharacterRecord record, out string reason)
    {
        if (record.PatronId == -1)
        {
            reason = "PatronId -1 marked this character as the monarch/root";
            return "monarch";
        }
        if (record.IsMule)
        {
            reason = "IsMule was true";
            return "mule";
        }
        if (record.IsRescueSquad)
        {
            reason = "IsRescueSquad was true";
            return "rescue";
        }
        if (record.IsPk)
        {
            reason = "IsPk was true";
            return "pk";
        }

        reason = "No displayed special tree icon flags were true";
        return "normal";
    }


    private static bool IsDiagnosticsEnabled(string key)
    {
        var envValue = Environment.GetEnvironmentVariable("ACGM_DIAGNOSTICS_" + key.ToUpperInvariant());
        if (IsTruthy(envValue))
            return true;

        var normalizedKey = NormalizeIniKey(key);

        try
        {
            var iniPath = Path.Combine(AppContext.BaseDirectory, "acgm.ini");
            if (!File.Exists(iniPath))
                return false;

            var inDiagnosticsSection = false;
            foreach (var rawLine in File.ReadLines(iniPath))
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";"))
                    continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    var sectionName = line[1..^1].Trim();
                    inDiagnosticsSection = string.Equals(sectionName, "Diagnostics", StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (!inDiagnosticsSection)
                    continue;

                var equals = line.IndexOf('=');
                if (equals < 0)
                    continue;

                var name = NormalizeIniKey(line[..equals]);
                var value = line[(equals + 1)..].Trim();

                if (string.Equals(name, normalizedKey, StringComparison.OrdinalIgnoreCase) && IsTruthy(value))
                    return true;
            }
        }
        catch
        {
            // Certification switches must never interfere with normal operation.
        }

        return false;
    }

    private static bool IsEnabled(string key)
    {
        var envValue = Environment.GetEnvironmentVariable("ACGM_" + key);
        if (IsTruthy(envValue))
            return true;

        try
        {
            var iniPath = Path.Combine(AppContext.BaseDirectory, "acgm.ini");
            if (!File.Exists(iniPath))
                return false;

            foreach (var rawLine in File.ReadLines(iniPath))
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";") || line.StartsWith("["))
                    continue;

                var equals = line.IndexOf('=');
                if (equals < 0)
                    continue;

                var name = NormalizeIniKey(line[..equals]);
                var value = line[(equals + 1)..].Trim();
                var normalizedKey = NormalizeIniKey(key);

                if (string.Equals(name, normalizedKey, StringComparison.OrdinalIgnoreCase) && IsTruthy(value))
                    return true;
            }
        }
        catch
        {
            // Certification switches must never interfere with normal operation.
        }

        return false;
    }

    private static string NormalizeIniKey(string key) => key.Trim().Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

    private static bool IsTruthy(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        value = value.Trim();
        return value.Equals("1", StringComparison.OrdinalIgnoreCase)
            || value.Equals("true", StringComparison.OrdinalIgnoreCase)
            || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
            || value.Equals("on", StringComparison.OrdinalIgnoreCase)
            || value.Equals("enabled", StringComparison.OrdinalIgnoreCase);
    }

    private static void WriteCharacterValueComparison(StringBuilder builder, string name, object? original, object? updated)
    {
        var before = Convert.ToString(original) ?? string.Empty;
        var after = Convert.ToString(updated) ?? string.Empty;
        var changed = string.Equals(before, after, StringComparison.Ordinal) ? "unchanged" : "changed";
        builder.AppendLine($"  {name}: before='{before}' after='{after}' ({changed})");
    }

    private static string Get(string[] fields, int index) => index >= 0 && index < fields.Length ? fields[index] : "<missing>";

    private static void TryWrite(Action<StringBuilder> build) => TryWrite(CharacterDiagnosticsPath, build);

    private static void TryWrite(string path, Action<StringBuilder> build)
    {
        try
        {
            var builder = new StringBuilder();
            build(builder);
            lock (SyncRoot)
            {
                File.AppendAllText(path, builder.ToString());
            }
        }
        catch
        {
            // Diagnostics must never interfere with ACGM legacy workflows.
        }
    }
}
