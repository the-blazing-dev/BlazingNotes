using BlazingNotes.Logic.Search;

namespace BlazingNotes.Logic.Services;

public static class NoteSearcher
{
    public static (bool longEnough, BzSearchQuery query, ICollection<Note> results) Search(ICollection<Note> notes,
        string searchTerm)
    {
        var query = new BzSearchQuery(searchTerm);
        if (!IsSearchTermLongEnough(searchTerm)) return (false, query, notes);

        var matches = notes.Where(x => BzSearch.IsMatch(query, x.Text)).ToList();

        return (true, query, matches);
    }

    public static bool IsSearchTermLongEnough(string search)
    {
        return search.Trim().Length >= 3;
    }
}