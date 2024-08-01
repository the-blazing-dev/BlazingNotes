namespace BlazingNotes.Logic.Tests.State;

public class AppStateUntaggedTests : TestBase
{
    public AppStateUntaggedTests(ITestOutputHelper helper) : base(helper)
    {
        Sut = Services.GetRequiredService<IState<AppState>>();
    }

    private IState<AppState> Sut { get; }

    [Fact]
    public void GetUntaggedNotes_ContainsNotesWithoutTags()
    {
        CreateThreeNotes();
        Dispatch(new NoteActions.CreateNoteRequestAction("my note without tags"));
        Dispatch(new NoteActions.CreateNoteRequestAction("another #tagged note"));

        var notes = Sut.Value.GetUntaggedNotes();
        notes.Should().ContainSingle(x => x.Text == "my note without tags");
    }

    [Fact]
    public void UntaggedNotes_DoesNotContainTrashedNotes()
    {
        var note1 = CreateNote("first note without tags");
        var note2 = CreateNote("second note without tags");

        Dispatch(new NoteActions.TrashNoteAction(note1.Id));

        Sut.Value.GetUntaggedNotes().Should().HaveCount(1).And.Contain(note2);
    }

    [Fact]
    public void JustCreatedNotesAreListedFirst()
    {
        var note1 = CreateNote("first note without tags");
        var note2 = CreateNote("second note without tags");
        var note3 = CreateNote("third note without tags");

        var ids = Sut.Value.GetUntaggedNotes().Select(x => x.Id).ToList();
        ids.Should().BeEquivalentTo([note3.Id, note2.Id, note1.Id], opt => opt.WithStrictOrdering());
    }
}