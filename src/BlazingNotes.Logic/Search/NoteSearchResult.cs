namespace BlazingNotes.Logic.Search;

public record NoteSearchResult(bool LongEnough, BzSearchQuery Query, ICollection<Note> Notes)
{
    public (ICollection<Note> notArchived, ICollection<Note> archived) SplitByArchived()
    {
        var archived = Notes.Where(x => x.ArchivedAt != null).ToList();
        var notArchived = Notes.Where(x => x.ArchivedAt == null).ToList();
        return (notArchived, archived);
    }
}