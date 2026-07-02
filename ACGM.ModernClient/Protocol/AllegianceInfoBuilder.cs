using ACGM.ModernClient.Models;

namespace ACGM.ModernClient.Protocol;

public static class AllegianceInfoBuilder
{
    public static AllegianceInfo Build(CharacterDetails details, CharacterRecord selected, IReadOnlyDictionary<int, CharacterRecord> recordsById)
    {
        ArgumentNullException.ThrowIfNull(details);
        ArgumentNullException.ThrowIfNull(selected);
        ArgumentNullException.ThrowIfNull(recordsById);

        var directVassals = recordsById.Values
            .Where(record => record.PatronId == selected.Id)
            .OrderBy(record => record.Name)
            .ToList();

        var descendants = new List<CharacterRecord>();
        AddDescendants(selected.Id, recordsById, descendants, new HashSet<int>());

        var path = details.PathToMonarch.Trim();
        var monarch = ResolveMonarch(path, selected, recordsById);
        var patron = FirstNonEmpty(details.Patron, selected.PatronName, ResolvePatronName(selected, recordsById));

        return new AllegianceInfo
        {
            CharacterId = selected.Id,
            CharacterName = FirstNonEmpty(details.Name, selected.Name),
            Patron = patron,
            Monarch = monarch,
            PathToMonarch = path,
            DirectVassalCount = directVassals.Count,
            TotalKnownVassalCount = descendants.Count,
            DirectVassals = directVassals,
            KnownDescendants = descendants.OrderBy(record => record.Name).ToList(),
            Notes = BuildNotes(details, selected, directVassals.Count, descendants.Count),
            SourceSummary = "Allegiance values are assembled from the character detail response plus the downloaded tree."
        };
    }

    private static void AddDescendants(int parentId, IReadOnlyDictionary<int, CharacterRecord> recordsById, List<CharacterRecord> output, HashSet<int> visited)
    {
        if (!visited.Add(parentId))
            return;

        var children = recordsById.Values
            .Where(record => record.PatronId == parentId)
            .OrderBy(record => record.Name)
            .ToList();

        foreach (var child in children)
        {
            output.Add(child);
            AddDescendants(child.Id, recordsById, output, visited);
        }
    }

    private static string ResolvePatronName(CharacterRecord selected, IReadOnlyDictionary<int, CharacterRecord> recordsById)
    {
        if (selected.PatronId > 0 && recordsById.TryGetValue(selected.PatronId, out var patron))
            return patron.Name;
        return string.Empty;
    }

    private static string ResolveMonarch(string pathToMonarch, CharacterRecord selected, IReadOnlyDictionary<int, CharacterRecord> recordsById)
    {
        if (!string.IsNullOrWhiteSpace(pathToMonarch))
        {
            var pieces = pathToMonarch
                .Split(new[] { "->", ">", "|", ",", "\\r", "\\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(piece => !string.IsNullOrWhiteSpace(piece))
                .ToList();
            if (pieces.Count > 0)
                return pieces[^1];
        }

        var current = selected;
        var visited = new HashSet<int>();
        while (current.PatronId > 0 && recordsById.TryGetValue(current.PatronId, out var patron) && visited.Add(current.Id))
            current = patron;

        return current.Name;
    }

    private static string BuildNotes(CharacterDetails details, CharacterRecord selected, int directVassalCount, int totalKnownVassalCount)
    {
        var lines = new List<string>();
        if (!string.IsNullOrWhiteSpace(details.FollowerCount))
            lines.Add($"Legacy follower count: {details.FollowerCount}");
        lines.Add($"Direct vassals in downloaded tree: {directVassalCount}");
        lines.Add($"Known descendants in downloaded tree: {totalKnownVassalCount}");
        if (selected.ComputedRank > 0)
            lines.Add($"Computed rank from tree: {selected.ComputedRank}");
        if (!string.IsNullOrWhiteSpace(details.PathToMonarch))
            lines.Add("Path to monarch provided by server detail response.");
        else
            lines.Add("Path to monarch inferred from downloaded tree because the detail response did not include a path.");
        return string.Join(Environment.NewLine, lines);
    }

    private static string FirstNonEmpty(params string[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }
        return string.Empty;
    }
}
