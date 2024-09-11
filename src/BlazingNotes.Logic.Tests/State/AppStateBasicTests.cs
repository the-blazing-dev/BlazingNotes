namespace BlazingNotes.Logic.Tests.State;

public class AppStateBasicTests : TestBase
{
    public AppStateBasicTests(ITestOutputHelper helper) : base(helper)
    {
        Sut = Services.GetRequiredService<IState<AppState>>();
    }

    private IState<AppState> Sut { get; }

    [Fact]
    public async Task PersistedNotesAreLoadedOnStoreInitialization()
    {
        Sut.Value.Notes.Should().BeEmpty();

        var note1 = new Note { Text = "Note1" };
        var note2 = new Note { Text = "Note2" };

        await ExecuteOnDb(async db =>
        {
            db.Add(note1);
            db.Add(note2);
            await db.SaveChangesAsync();
        });

        var action = Activator.CreateInstance(typeof(StoreInitializedAction), true)!;
        Dispatch(action);

        Sut.Value.Notes.Should().HaveCount(2);
        Sut.Value.Notes.Should().ContainEquivalentOf(note1);
        Sut.Value.Notes.Should().ContainEquivalentOf(note2);
    }

    [Fact]
    public void CreateNoteActionReducer_AddsNoteToCorrespondingStateProperty()
    {
        Sut.Value.Notes.Should().BeEmpty();

        var action = new NoteActions.CreateNoteRequestAction("first note");
        Dispatch(action);

        var note = Sut.Value.Notes.Single();
        note.Text.Should().Be("first note");
        note.CreatedAt.Should().BeCloseToNow();
    }

    [Fact]
    public void CreateNoteActionReducer_TrimsText()
    {
        var action = new NoteActions.CreateNoteRequestAction("  first note\n");
        Dispatch(action);

        Sut.Value.Notes.Should().ContainSingle(x => x.Text == "first note");
    }

    [Fact]
    public void CreateNoteActionReducer_IgnoresEmptyNotes()
    {
        var action = new NoteActions.CreateNoteRequestAction("  \n");
        Dispatch(action);

        Sut.Value.Notes.Should().BeEmpty();
    }

    [Fact]
    public void NotesCanBeCreatedAndArchivedInOneStep()
    {
        Dispatch(new NoteActions.CreateNoteRequestAction("some note", IsArchived: true));
        var note = Sut.Value.Notes.Single();
        note.Text.Should().Be("some note");
        note.ArchivedAt.Should().BeCloseToNow();
    }

    [Fact]
    public void ShowCreateNoteDialog()
    {
        Dispatch(new NoteActions.ShowCreateNoteDialogAction());

        Sut.Value.ShowCreateNoteDialog.Should().BeTrue();

        Dispatch(new NoteActions.HideCreateNoteDialogAction());

        Sut.Value.ShowCreateNoteDialog.Should().BeFalse();

        // now open + save
        Dispatch(new NoteActions.ShowCreateNoteDialogAction());

        Sut.Value.ShowCreateNoteDialog.Should().BeTrue();

        Dispatch(new NoteActions.CreateNoteRequestAction("hello"));

        Sut.Value.ShowCreateNoteDialog.Should().BeFalse();
        Sut.Value.Notes.Should().ContainSingle(x => x.Text == "hello");
    }

    [Fact]
    public void NewlyCreatedNotesAreListedFirstInHomePage()
    {
        Dispatch(new NoteActions.CreateNoteRequestAction("first note"));
        Thread.Sleep(1);
        Dispatch(new NoteActions.CreateNoteRequestAction("second note"));

        var notes = Sut.Value.GetHomePageNotes().ToList();
        notes[0].Text.Should().Be("second note"); // second note was created later and is therefore "fresher"
        notes[1].Text.Should().Be("first note");
    }

    [Fact]
    public void OnlyTenItemsAreShownInHomePage()
    {
        for (var i = 0; i < 15; i++) Dispatch(new NoteActions.CreateNoteRequestAction("Note #" + i));

        Sut.Value.GetHomePageNotes().Should().HaveCount(10);
    }

    [Fact]
    public void NoteEditingWithSaving()
    {
        var (note1, _, _) = CreateThreeNotes();
        Dispatch(new NoteActions.StartNoteEditingAction(note1));

        Sut.Value.CurrentlyEditingNote.Should().Be(note1);

        Dispatch(new NoteActions.SaveNoteEditingAction(note1, "Note1 #numberOne"));

        Sut.Value.CurrentlyEditingNote.Should().BeNull();
        var note1Fresh = Sut.Value.Notes.Single(x => x.Text.Contains("Note1"));
        note1Fresh.Id.Should().Be(note1.Id);
        note1Fresh.Text.Should().Be("Note1 #numberOne");
    }

    [Fact]
    public void NoteEditingWithCancel()
    {
        var (note1, _, _) = CreateThreeNotes();
        var originalText = note1.Text;
        Dispatch(new NoteActions.StartNoteEditingAction(note1));

        Sut.Value.CurrentlyEditingNote.Should().Be(note1);

        Dispatch(new NoteActions.CancelNoteEditingAction(note1));

        Sut.Value.CurrentlyEditingNote.Should().BeNull();
        var note1Fresh = Sut.Value.Notes.Single(x => x.Text.Contains("Note1"));
        note1Fresh.Id.Should().Be(note1.Id);
        note1Fresh.Text.Should().Be(originalText);
    }

    [Theory]
    [InlineData("Note1 #first", "Note1 #first")] // no real update
    [InlineData("\nNote1 #updated  ", "Note1 #updated")] // trimming
    [InlineData("\nNote1 #first  ", "Note1 #first")] // trimming
    [InlineData("\n  ", "Note1 #first")] // is ignored for now
    public void HandlesNoteEditingProperly(string textBoxInput, string expectedEntityText)
    {
        var (note1, _, _) = CreateThreeNotes();
        Dispatch(new NoteActions.StartNoteEditingAction(note1));
        Sut.Value.CurrentlyEditingNote.Should().Be(note1);

        Dispatch(new NoteActions.SaveNoteEditingAction(note1, textBoxInput));

        Sut.Value.CurrentlyEditingNote.Should().BeNull();
        var note1Fresh = Sut.Value.Notes.Single(x => x.Text.Contains("Note1"));
        note1Fresh.Id.Should().Be(note1.Id);
        note1Fresh.Text.Should().Be(expectedEntityText);
    }

    [Fact]
    public void ModifiedAt_IsSet_WhenNoteIsEdited()
    {
        var (note1, _, _) = CreateThreeNotes();
        note1.ModifiedAt.Should().BeNull();
        Thread.Sleep(1);
        Dispatch(new NoteActions.SaveNoteEditingAction(note1, note1.Text + " #updated"));
        var note1Fresh = Sut.Value.Notes.Single(x => x.Id == note1.Id);
        note1Fresh.ModifiedAt.Should().BeCloseToNow();
        note1Fresh.ModifiedAt.Should().BeAfter(note1Fresh.CreatedAt);

        // check if it detects "real" changes
        var modifiedAtBackup = note1Fresh.ModifiedAt;
        Thread.Sleep(1);
        Dispatch(new NoteActions.SaveNoteEditingAction(note1Fresh, note1Fresh.Text)); // no change
        note1Fresh = Sut.Value.Notes.Single(x => x.Id == note1Fresh.Id);
        note1Fresh.ModifiedAt.Should().Be(modifiedAtBackup);
    }

    [Fact]
    public void GetTags_ReturnsCollectionOfTags()
    {
        CreateThreeNotes();
        var tags = Sut.Value.GetTags();
        tags.Should().HaveCount(4);
        tags.Should().Contain(["#first", "#second", "#third", "#last"]);
    }

    [Fact]
    public void GetTags_ContainsUniqueItems_AndIsCaseInsensitive()
    {
        Dispatch(new NoteActions.CreateNoteRequestAction("#important note"));
        Dispatch(new NoteActions.CreateNoteRequestAction("#IMPORTANT note 2"));

        var tags = Sut.Value.GetTags();
        tags.Should().ContainSingle("#importANT");
    }
}