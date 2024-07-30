namespace BlazingNotes.Logic.Services;

public static class NoteSearcher
{
    public static (bool longEnough, string highlightText, ICollection<Note> results) Search(ICollection<Note> notes,
        string searchTerm)
    {
        if (!IsSearchTermLongEnough(searchTerm)) return (false, "", notes);

        var search = searchTerm.Trim();
        var matches = notes.Where(x => x.Text.ContainsIgnoreCase(searchTerm)).ToList();
        return (true, search, matches);
    }

    public static bool IsSearchTermLongEnough(string search)
    {
        return search.Trim().Length > 2;
    }
}