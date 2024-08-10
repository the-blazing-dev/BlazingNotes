namespace BlazingNotes.Logic.Search;

public struct BzSearchQuery
{
    public BzSearchQuery(string query)
    {
        RequiredTerms = query.BzSplit(' ').ToImmutableArray();
    }

    public IReadOnlyCollection<string> RequiredTerms { get; }
}