namespace BlazingNotes.Logic.Tests.State;

public class AppStateArchivingTests : TestBase
{
    public AppStateArchivingTests(ITestOutputHelper helper) : base(helper)
    {
        Sut = Services.GetRequiredService<IState<AppState>>();
    }

    public IState<AppState> Sut { get; set; }

    [Fact]
    public async Task AllNotes_IncludingArchived_AreLoadedOnAppStartup()
    {
        // good enough for now, see comment in NoteEffects.cs

        await ExecuteOnDb(async db =>
        {
            db.Notes.Add(new Note { Text = "I'm archived", ArchivedAt = DateTime.UtcNow });
            db.Notes.Add(new Note { Text = "I'm not archived", ArchivedAt = null });
            await db.SaveChangesAsync();
        });

        var action = Activator.CreateInstance(typeof(StoreInitializedAction), true);
        Dispatch(action!);

        Sut.Value.Notes.Should().HaveCount(2);
        Sut.Value.GetHomePageNotes().Should().HaveCount(1).And.OnlyContain(x => x.ArchivedAt == null);
        Sut.Value.GetArchivedNotes().Should().HaveCount(1).And.OnlyContain(x => x.ArchivedAt != null);
    }

    [Fact]
    public async Task ArchiveNote_UpdatesDbAccordingly()
    {
        var (_, note2, _) = CreateThreeNotes();
        Dispatch(new NoteActions.ArchiveNoteAction(note2.Id));

        await ExecuteOnDb(async db =>
        {
            var entity = await db.Notes.FindAsync(note2.Id);
            entity.ArchivedAt.Should().BeCloseToNow();
        });
    }

    [Fact]
    public void ArchivedNotesAreNotListedInHomePage()
    {
        CreateThreeNotes();
        var someNote = Sut.Value.Notes.First();
        Dispatch(new NoteActions.ArchiveNoteAction(someNote.Id));

        Sut.Value.GetHomePageNotes().Should().NotContain(x => x.Id == someNote.Id);
    }

    [Fact]
    public async Task ArchiveNote_DoesNotSetModifiedAt()
    {
        var (_, note2, _) = CreateThreeNotes();
        Dispatch(new NoteActions.ArchiveNoteAction(note2.Id));

        await ExecuteOnDb(async db =>
        {
            var entity = await db.Notes.FindAsync(note2.Id);
            entity.ArchivedAt.Should().BeCloseToNow();
            entity.ModifiedAt.Should().BeNull(); // not wanted for now
        });
    }

    [Fact]
    public void ArchiveNote_KeepsTheNoteInTheState() // for now
    {
        var (_, note2, _) = CreateThreeNotes();
        Dispatch(new NoteActions.ArchiveNoteAction(note2.Id));

        Sut.Value.Notes.Should().Contain(x => x.Id == note2.Id && x.ArchivedAt.HasValue);
    }

    [Fact]
    public async Task ArchivedNotesCanBeRestored()
    {
        var (_, note2, _) = CreateThreeNotes();
        Dispatch(new NoteActions.ArchiveNoteAction(note2.Id));
        Dispatch(new NoteActions.RestoreNoteFromArchiveAction(note2.Id));

        await ExecuteOnDb(async db =>
        {
            var entity = await db.Notes.FindAsync(note2.Id);
            entity.ArchivedAt.Should().BeNull();
        });

        Sut.Value.Notes.Should().Contain(x => x.Id == note2.Id && x.ArchivedAt == null);
    }

    [Fact]
    public void ArchivedNotes_JustArchivedItemsAreListedFirst()
    {
        var (note1, note2, note3) = CreateThreeNotes();
        Dispatch(new NoteActions.ArchiveNoteAction(note2.Id));
        Thread.Sleep(1);
        Dispatch(new NoteActions.ArchiveNoteAction(note3.Id));
        Thread.Sleep(1);
        Dispatch(new NoteActions.ArchiveNoteAction(note1.Id));

        var archivedIds = Sut.Value.GetArchivedNotes().Select(x => x.Id).ToList();
        archivedIds.Should().BeEquivalentTo([note1.Id, note3.Id, note2.Id], opt => opt.WithStrictOrdering());
    }

    [Fact]
    public void ArchivedNotes_AreVisibleInSearchableNotes()
    {
        var (_, _, note3) = CreateThreeNotes();
        Dispatch(new NoteActions.ArchiveNoteAction(note3.Id));

        var result = Sut.Value.GetSearchableNotes();
        result.Should().HaveCount(3);
    }
}