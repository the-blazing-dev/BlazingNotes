namespace BlazingNotes.Logic.Search;

public static class NoteSearcher
{
    public static NoteSearchResult Search(ICollection<Note> notes,
        string searchTerm)
    {
        var query = new BzSearchQuery(searchTerm);
        if (!IsSearchTermLongEnough(searchTerm))
        {
            return new NoteSearchResult(false, query, notes);
        }

        var matches = notes.Where(x => BzSearch.IsMatch(query, x.Text)).ToList();

        return new NoteSearchResult(true, query, matches);
    }

    public static bool IsSearchTermLongEnough(string search)
    {
        return search.Trim().Length >= 3;
    }
}