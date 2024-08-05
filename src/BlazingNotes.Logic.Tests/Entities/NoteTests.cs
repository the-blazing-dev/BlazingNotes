using BlazingDev.BlazingExtensions;

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
    [InlineData("Deutsche #Wörter machen #Spaß", "#Wörter #Spaß")]
    [InlineData("#Äpfel fallen vom Baum", "#Äpfel")]
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
}