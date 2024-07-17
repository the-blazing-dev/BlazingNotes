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
}