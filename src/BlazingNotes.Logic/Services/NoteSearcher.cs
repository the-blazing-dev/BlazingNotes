namespace BlazingNotes.Logic.Services;

public static class NoteSearcher
{
    public static (bool longEnough, string highlightText, ICollection<Note> results) Search(ICollection<Note> notes,
        string searchTerm)
    {
        var search = searchTerm.Trim();
        if (search.Length <= 2) return (false, "", notes);

        var matches = notes.Where(x => x.Text.ContainsIgnoreCase(searchTerm)).ToList();
        return (true, search, matches);
    }
}