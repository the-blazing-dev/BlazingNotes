namespace BlazingNotes.Logic.State;

[FeatureState]
public record AppState
{
    public required ImmutableList<Note> Notes { get; init; } = ImmutableList<Note>.Empty;
    public Note? CurrentlyEditingNote { get; init; }
    public bool ShowCreateNoteDialog { get; set; }

    public ICollection<Note> GetHomePageNotes()
    {
        return Notes.Where(x => !x.IsArchived)
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .ToList();
    }

    public ICollection<Note> GetArchivedNotes()
    {
        return Notes.Where(x => x.IsArchived)
            .OrderByDescending(x => x.ArchivedAt)
            .ToList();
    }
}