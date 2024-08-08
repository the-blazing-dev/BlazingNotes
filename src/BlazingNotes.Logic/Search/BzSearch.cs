namespace BlazingNotes.Logic.Search;

public static class BzSearch
{
    public static bool IsMatch(string fullText, string query)
    {
        if (fullText.LacksContent() ||
            query.LacksContent())
        {
            return false;
        }

        var queryParts = query.BzSplit(' ');
        foreach (var queryPart in queryParts)
        {
            var notFound = !fullText.ContainsIgnoreCase(queryPart);
            if (notFound)
            {
                // early exit as soon as some term is not found
                return false;
            }
        }

        return true;
    }
}