using BlazingNotes.Logic.Entities;
using BlazingNotes.Logic.State;
using BlazingNotes.Logic.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;
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

    [Fact]
    public async Task NotesCanBeTrashed()
    {
        (_, _, var note) = CreateThreeNotes();
        Dispatch(new NoteActions.TrashNoteAction(note.Id));

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
        await NotesCanBeTrashed();

        var deletedNote = Sut.Value.GetDeletedNotes().Single();
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
    public async Task TrashedNotes_AreNotShownInArchivedPage()
    {
        (_, _, var note) = CreateThreeNotes();
        Dispatch(new NoteActions.ArchiveNoteAction(note.Id));
        Dispatch(new NoteActions.TrashNoteAction(note.Id));

        Sut.Value.GetArchivedNotes().Should().NotContain(x => x.Id == note.Id);
    }

    [Fact]
    public async Task NotesCanBeRestoredFromTrash()
    {
        await NotesCanBeTrashed();
        var deletedNote = Sut.Value.GetDeletedNotes().Single();

        Dispatch(new NoteActions.RestoreNoteFromTrashAction(deletedNote.Id));

        Sut.Value.GetDeletedNotes().Should().BeEmpty();
    }

    [Fact]
    public void JustTrashedNotesAreListedFirst()
    {
        var (note1, note2, note3) = CreateThreeNotes();
        Dispatch(new NoteActions.TrashNoteAction(note2.Id));
        Thread.Sleep(1);
        Dispatch(new NoteActions.TrashNoteAction(note3.Id));
        Thread.Sleep(1);
        Dispatch(new NoteActions.TrashNoteAction(note1.Id));

        var ids = Sut.Value.GetDeletedNotes().Select(x => x.Id).ToList();
        ids.Should().BeEquivalentTo([note1.Id, note3.Id, note2.Id], opt => opt.WithStrictOrdering());
    }

    [Fact]
    public async Task NotesCanBeDeletedPermanently()
    {
        var (note1, note2, note3) = CreateThreeNotes();
        // note 2 is trashed first and then deleted permanently
        Dispatch(new NoteActions.TrashNoteAction(note2.Id));
        Dispatch(new NoteActions.DeleteNotePermanentlyAction(note2.Id));
        // note 3 is deleted permanently immediately
        Dispatch(new NoteActions.DeleteNotePermanentlyAction(note3.Id));

        Sut.Value.Notes.Should().HaveCount(1).And.ContainSingle(x => x == note1);

        await ExecuteOnDb(async db =>
        {
            var count = await db.Notes.CountAsync();
            count.Should().Be(1);
        });
    }
}