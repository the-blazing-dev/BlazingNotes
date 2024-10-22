using BlazingDev.BlazingExtensions;
using BlazingNotes.Logic.Search;

namespace BlazingNotes.Logic.Tests.Entities;

public class NoteTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("no tags", "")]
    [InlineData("one #cool tag", "#cool")]
    [InlineData("two #tags must #also work", "#tags #also")]
    [InlineData("with #12numbers34", "#12numbers34")]
    [InlineData("with #dash-and_underline", "#dash-and_underline")]
    [InlineData("with #slash/does not work", "#slash")]
    [InlineData("I am a C#-developer", "")]
    [InlineData("inside#text", "")]
    [InlineData("inside-#text", "")]
    [InlineData("Deutsche #Wörter machen #Spaß", "#Wörter #Spaß")]
    [InlineData("#Äpfel fallen vom Baum", "#Äpfel")]
    [InlineData("HTML-encoded&#xD;&#xA;newline", "")]
    public void GetTags(string text, string expectedSpaceSeparated)
    {
        var note = new Note
        {
            Text = text
        };
        note.GetTags().StringJoin(" ").Should().Be(expectedSpaceSeparated);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("no tags", "no tags")]
    [InlineData("one #cool tag", "one <bz-tag>#cool</bz-tag> tag")]
    [InlineData("two #tags must #also work", "two <bz-tag>#tags</bz-tag> must <bz-tag>#also</bz-tag> work")]
    [InlineData("with #12numbers34", "with <bz-tag>#12numbers34</bz-tag>")]
    [InlineData("with #dash-and_underline", "with <bz-tag>#dash-and_underline</bz-tag>")]
    [InlineData("with #slash/does not work", "with <bz-tag>#slash</bz-tag>/does not work")]
    public void GetTextWithTagsMarked(string text, string expected)
    {
        var note = new Note
        {
            Text = text
        };
        note.GetTextWithTagsMarked().Should().Be(expected);
    }

    [Theory]
    [InlineData("hello world", "", "hello world")]
    [InlineData("hello world", "world", "hello <bz-search>world</bz-search>")]
    [InlineData("hello world", "  world  ", "hello <bz-search>world</bz-search>")] // search trimming
    [InlineData("hello WORld", "woRLD", "hello <bz-search>WORld</bz-search>")] // keep original casing
    [InlineData("hello world", "hello world",
        "<bz-search>hello</bz-search> <bz-search>world</bz-search>")] // or the other way
    [InlineData("hello world", "world hello",
        "<bz-search>hello</bz-search> <bz-search>world</bz-search>")] // or the other way
    [InlineData("<xss>", "", "&lt;xss&gt;")]
    [InlineData("<xss>", "xss", "&lt;<bz-search>xss</bz-search>&gt;")]
    [InlineData("<xss>", "<xss>", "<bz-search>&lt;xss&gt;</bz-search>")]
    [InlineData("<xss>", "xss>", "&lt;<bz-search>xss&gt;</bz-search>")]
    [InlineData("<xss>", "s> <x", "<bz-search>&lt;x</bz-search>s<bz-search>s&gt;</bz-search>")]
    [InlineData("hello #world", "", "hello <bz-tag>#world</bz-tag>")]
    [InlineData("hello #world", "hello", "<bz-search>hello</bz-search> <bz-tag>#world</bz-tag>")]
    [InlineData("hello #world", "#world", "hello <bz-search><bz-tag>#world</bz-tag></bz-search>")] // oder anders rum
    [InlineData("hello #world", "lo #world",
        "hel<bz-search>lo</bz-search> <bz-search><bz-tag>#world</bz-tag></bz-search>")] // or the other way
    [InlineData("hello #world", "#wor",
        "hello <bz-search><bz-tag>#wor</bz-tag></bz-search>ld")] // optimization: "ld" still as <bz-tag>
    [InlineData("hello #world", "rld", "hello <bz-tag>#wo</bz-tag><bz-search>rld</bz-search>")]
    // no false positives when searching for the wrapper tags
    [InlineData("hello #world", "hello tag", "<bz-search>hello</bz-search> <bz-tag>#world</bz-tag>")]
    [InlineData("hello #world", "hello search", "<bz-search>hello</bz-search> <bz-tag>#world</bz-tag>")]
    public void XssSafeSearch(string noteText, string search, string expectedOutput)
    {
        var bzSearch = new BzSearchQuery(search);
        var note = new Note { Text = noteText };
        var actual = note.GetXssSafeText(bzSearch);
        actual.Should().Be(expectedOutput);
    }
}