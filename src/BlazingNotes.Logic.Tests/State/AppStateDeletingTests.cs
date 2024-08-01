using BlazingNotes.Logic.Entities;
using BlazingNotes.Logic.State;
using BlazingNotes.Logic.Tests.TestSupport;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace BlazingNotes.Logic.Tests.State;

public class AppStateDeletingTests : TestBase
{
    public AppStateDeletingTests(ITestOutputHelper helper) : base(helper)
    {
        Sut = Services.GetRequiredService<IState<AppState>>();
    }

    private IState<AppState> Sut { get; set; }

    private List<Note> GetDeletedNotes()
    {
        return Sut.Value.Notes.Where(x => x.DeletedAt.HasValue).ToList();
    }


    [Fact]
    public async Task NotesCanBeDeleted()
    {
        (_, _, var note) = CreateThreeNotes();
        Dispatch(new NoteActions.DeleteNoteAction(note.Id));

        Sut.Value.Notes.Should().ContainSingle(x => x.Id == note.Id && x.DeletedAt.HasValue);

        await ExecuteOnDb(async db =>
        {
            var entity = await db.Notes.FindAsync(note.Id);
            entity.DeletedAt.Should().BeCloseToNow();
            entity.ModifiedAt.Should().BeNull(); // not wanted for now
        });
    }

    [Fact]
    public async Task DeletedNotes_AreNotListedInHomePage()
    {
        await NotesCanBeDeleted();

        var deletedNote = GetDeletedNotes().Single();
        Sut.Value.GetHomePageNotes().Should().HaveCount(2);
        Sut.Value.GetHomePageNotes().Should().NotContain(x => x.Id == deletedNote.Id);
    }

    [Fact]
    public async Task DeletedNotes_AreLoadedOnAppStart()
    {
        await ExecuteOnDb(async db =>
        {
            db.Notes.Add(new Note
            {
                Text = "I am deleted",
                DeletedAt = DateTime.UtcNow
            });
            db.Notes.Add(new Note
            {
                Text = "I am not deleted"
            });
            await db.SaveChangesAsync();
        });

        var action = Activator.CreateInstance(typeof(StoreInitializedAction), true)!;
        Dispatch(action);

        Sut.Value.Notes.Should().HaveCount(2);
        Sut.Value.Notes.Should().ContainSingle(x => x.Text == "I am deleted" && x.DeletedAt != null);
        Sut.Value.Notes.Should().ContainSingle(x => x.Text == "I am not deleted" && x.DeletedAt == null);
    }

    [Fact]
    public async Task DeletedNotes_AreNotShownInArchivedPage()
    {
        (_, _, var note) = CreateThreeNotes();
        Dispatch(new NoteActions.ArchiveNoteAction(note.Id));
        Dispatch(new NoteActions.DeleteNoteAction(note.Id));

        Sut.Value.GetArchivedNotes().Should().NotContain(x => x.Id == note.Id);
    }

    [Fact]
    public async Task NotesCanBeRestoredFromTrash()
    {
        await NotesCanBeDeleted();
        var deletedNote = GetDeletedNotes().Single();

        Dispatch(new NoteActions.RestoreNoteFromTrashAction(deletedNote.Id));

        GetDeletedNotes().Should().BeEmpty();
    }
}