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