using BlazingNotes.Logic.Search;

namespace BlazingNotes.Logic.Tests.Search;

public class BzSearchTests
{
    [Theory]
    [InlineData("NOTE", true)]
    [InlineData("note", true)] // case insensitive, as options?
    [InlineData("#important", true)] // tag
    [InlineData("#import", true)] // partial tag
    [InlineData("tosearch", false)] // not existing that way
    [InlineData("world", false)] // definitely not existing
    public void FindsResultsBySingleWord(string query, bool matches)
    {
        var input = "this is a NOTE that I WANT to search for #important";
        BzSearch.IsMatch(query, input).Should().Be(matches);
    }

    [Theory]
    [InlineData("NOTE that", true)] // subsequent
    [InlineData("note THAT", true)] // case insensitive
    [InlineData("that note", true)] // not subsequent
    [InlineData("not thi sea", true)] // partial words 
    [InlineData("note this want search #imp", true)]
    [InlineData("this notethat", false)] // not existing that way 
    [InlineData("this note x", false)] // letter not existing 
    [InlineData("this note (", false)] // character not existing 
    public void FindsResultsByMultipleTerms(string query, bool matches)
    {
        var input = "this is a NOTE that I WANT to search for #important";
        BzSearch.IsMatch(query, input).Should().Be(matches);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("", "   ")]
    [InlineData("   ", "   ")]
    [InlineData("\t\t", "\t")]
    [InlineData("\t\t", "")]
    [InlineData("\t\t", "\n")]
    public void NoMatchForEmptyQueriesOrEmptyTexts(string fullText, string query)
    {
        BzSearch.IsMatch(query, fullText).Should().Be(false);
    }

    [Theory]
    [InlineData("   hello world   ", "   hello   ", true)]
    [InlineData("hello world", "  hello   world  ", true)]
    [InlineData("  hello   world  ", "hello world", true)]
    public void HandlesTrimmableStringsProperly(string fullText, string query, bool isMatch)
    {
        BzSearch.IsMatch(query, fullText).Should().Be(isMatch);
    }
}