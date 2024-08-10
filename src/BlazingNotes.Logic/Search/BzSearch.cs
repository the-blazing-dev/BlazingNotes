namespace BlazingNotes.Logic.Search;

public static class BzSearch
{
    public static bool IsMatch(string query, string fullText)
    {
        if (fullText.LacksContent() ||
            query.LacksContent())
            return false;

        var queryAsStruct = new BzSearchQuery(query);
        return IsMatch(queryAsStruct, fullText);
    }

    public static bool IsMatch(BzSearchQuery query, string fullText)
    {
        if (fullText.LacksContent() ||
            query.RequiredTerms.LacksContent())
            return false;

        foreach (var queryPart in query.RequiredTerms)
        {
            var notFound = !fullText.ContainsIgnoreCase(queryPart);
            if (notFound)
                // early exit as soon as some term is not found
                return false;
        }

        return true;
    }
}