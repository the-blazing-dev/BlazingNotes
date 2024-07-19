namespace BlazingNotes.Logic.State;

[FeatureState]
public record AppState
{
    public required ImmutableList<Note> Notes { get; init; } = ImmutableList<Note>.Empty;
    public Note? CurrentlyEditingNote { get; init; }

    public IEnumerable<Note> GetHomePageNotes()
    {
        return Notes.Where(x => !x.IsArchived);
    }

    public IEnumerable<Note> GetArchivedNotes()
    {
        return Notes.Where(x => x.IsArchived);
    }
}