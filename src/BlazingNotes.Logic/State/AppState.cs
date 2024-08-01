namespace BlazingNotes.Logic.State;

[FeatureState]
public record AppState
{
    public required ImmutableList<Note> Notes { get; init; } = ImmutableList<Note>.Empty;
    public Note? CurrentlyEditingNote { get; init; }
    public bool ShowCreateNoteDialog { get; set; }

    public ICollection<Note> GetHomePageNotes()
    {
        return Notes
            .Where(x => x.ArchivedAt == null)
            .Where(x => x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .ToList();
    }

    public ICollection<Note> GetUntaggedNotes()
    {
        return Notes
            .Where(x => x.DeletedAt == null)
            .Where(x => x.GetTags().Count == 0)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public ICollection<Note> GetArchivedNotes()
    {
        return Notes
            .Where(x => x.ArchivedAt != null)
            .Where(x => x.DeletedAt == null) // deleted wins
            .OrderByDescending(x => x.ArchivedAt)
            .ToList();
    }

    public ICollection<Note> GetDeletedNotes()
    {
        return Notes
            .Where(x => x.DeletedAt.HasValue)
            .OrderByDescending(x => x.DeletedAt)
            .ToList();
    }

    public HashSet<string> GetTags()
    {
        // a short test in LinqPad resulted in 10ms for 10_000 notes which is fast enough for now
        // of course we need some better implementation / caching of notes as soon as the app is used more frequently
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var note in Notes)
        {
            var foundTags = note.GetTags();
            foundTags.ForEach(x => result.Add(x));
        }

        return result;
    }
}