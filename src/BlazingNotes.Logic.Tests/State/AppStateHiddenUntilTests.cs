namespace BlazingNotes.Logic.Tests.State;

public class AppStateHiddenUntilTests(ITestOutputHelper helper) : AppStateTestBase(helper)
{
    [Fact]
    public async Task NotesCanBeHiddenForSomeTime()
    {
        var (n1, n2, n3) = CreateThreeNotes();
        n1.HiddenUntil.Should().BeNull();
        var dur = TimeSpan.FromHours(4);

        Dispatch(new NoteActions.HideForAction(n1.Id, dur));

        n1 = Sut.GetNote(n1.Id);
        // use DateTime.Now (and not UtcNow) because the user wants it to be hidden from his point of view
        n1.HiddenUntil.Should().BeCloseTo(DateTime.Now.Add(dur));

        // hidden from home page
        Sut.Value.GetHomePageNotes().Should().NotContain(n1);

        // unhide note
        Dispatch(new NoteActions.UnhideAction(n1.Id));
        n1 = Sut.GetNote(n1.Id);
        n1.HiddenUntil.Should().BeNull();
        Sut.Value.GetHomePageNotes().Should().Contain(n1);
    }
}