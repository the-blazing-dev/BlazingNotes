using BlazingNotes.Infrastructure.Data;
using BlazingNotes.Logic.Entities;
using BlazingNotes.Logic.State;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace BlazingNotes.Logic.Tests.State;

public class AppStateTests : TestBase
{
    public AppStateTests(ITestOutputHelper helper) : base(helper)
    {
        Sut = Services.GetRequiredService<IState<AppState>>();
    }

    public IState<AppState> Sut { get; set; }

    [Fact]
    public async Task PersistedNotesAreLoadedOnStoreInitialization()
    {
        Sut.Value.Notes.Should().BeEmpty();

        var note1 = new Note { Text = "Note1" };
        var note2 = new Note { Text = "Note2" };

        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDb>();
            db.Add(note1);
            db.Add(note2);
            await db.SaveChangesAsync();
        }

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

        Sut.Value.Notes.Should().ContainSingle(x => x.Text == "first note");
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
    public void HandlesNoteEditingProperty(string textBoxInput, string expectedEntityText)
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

    private (Note note1, Note note2, Note note3) CreateThreeNotes()
    {
        Dispatch(new NoteActions.CreateNoteRequestAction("Note1 #first"));
        Dispatch(new NoteActions.CreateNoteRequestAction("Note2 #second"));
        Dispatch(new NoteActions.CreateNoteRequestAction("Note3 #third #last"));

        var notes = Sut.Value.Notes.OrderBy(x => x.Text).ToList();
        notes.Should().HaveCount(3);
        return (notes[0], notes[1], notes[2]);
    }
}