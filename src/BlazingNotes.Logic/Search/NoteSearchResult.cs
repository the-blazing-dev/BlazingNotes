namespace BlazingNotes.Logic.Search;

public record NoteSearchResult(bool LongEnough, BzSearchQuery Query, ICollection<Note> Notes)
{
    public (ICollection<Note> hidden, ICollection<Note> notArchived, ICollection<Note> archived) Split()
    {
        var archived = Notes.Where(x => x.ArchivedAt != null).ToList();
        var hidden = Notes.Except(archived).Where(x => x.IsHidden()).ToList();
        var notArchived = Notes.Except(hidden).Except(archived).ToList();
        return (hidden, notArchived, archived);
    }
}