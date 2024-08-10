using BlazingNotes.Infrastructure.Data;
using BlazingNotes.Logic.Entities;
using BlazingNotes.Logic.State;
using Fluxor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlazingNotes.Infrastructure.State;

public class NoteEffects(IDbContextFactory<AppDb> dbFactory, ILogger<NoteEffects> logger)
{
    [EffectMethod]
    public async Task Handle(StoreInitializedAction action, IDispatcher dispatcher)
    {
        // this is a very pragmatic approach: as the intention is to only store very small notes
        // and this app works with a local database, we can just load all notes into memory
        // which gives us advantages like instant search and faster UI experience
        // as soon as this will lead to too large memory consumption we have to rethink the approach

        await using var db = await dbFactory.CreateDbContextAsync();
        var notes = await db.Notes.AsNoTracking().ToListAsync();
        notes = await DeleteOldTrashedNotes(db, notes);

        dispatcher.Dispatch(new NoteActions.NotesLoadedAction(notes));
    }

    private async Task<List<Note>> DeleteOldTrashedNotes(AppDb db, List<Note> notes)
    {
        var trashClearLimit = DateTime.UtcNow.Date.AddDays(-30);
        var notesToRemove = notes.Where(n => n.DeletedAt <= trashClearLimit).ToList();
        if (notesToRemove.Any())
        {
            db.Notes.RemoveRange(notesToRemove);
            await db.SaveChangesAsync();
            logger.LogInformation("Finally deleted {Count} notes from db", notesToRemove.Count);
            notes = notes.Except(notesToRemove).ToList();
        }

        return notes;
    }

    [EffectMethod]
    public async Task Handle(NoteActions.CreateNoteRequestAction action, IDispatcher dispatcher)
    {
        if (!action.Text.HasContent()) return;

        var note = new Note
        {
            Text = action.Text.Trim()
        };

        Clean(note);

        await using var db = await dbFactory.CreateDbContextAsync();
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        dispatcher.Dispatch(new NoteActions.NoteCreatedAction(note));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.SaveNoteEditingAction action, IDispatcher dispatcher)
    {
        if (!action.NewText.HasContent())
        {
            // ignored for now
            dispatcher.Dispatch(new NoteActions.CancelNoteEditingAction(action.Note));
            return;
        }

        await using var db = await dbFactory.CreateDbContextAsync();
        var noteFresh = await db.Notes.FindRequiredAsync(action.Note.Id);
        noteFresh.Text = action.NewText;
        Clean(noteFresh);

        // only update entity + timestamp on "real" changes
        if (db.Entry(noteFresh).State == EntityState.Modified)
        {
            noteFresh.ModifiedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        dispatcher.Dispatch(new NoteActions.SaveNoteEditingSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.ArchiveNoteAction action, IDispatcher dispatcher)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var noteFresh = await db.Notes.FindRequiredAsync(action.NoteId);
        noteFresh.ArchivedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        dispatcher.Dispatch(new NoteActions.ArchiveNoteSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.RestoreNoteFromArchiveAction action, IDispatcher dispatcher)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var noteFresh = await db.Notes.FindRequiredAsync(action.NoteId);
        noteFresh.ArchivedAt = null;
        await db.SaveChangesAsync();

        dispatcher.Dispatch(new NoteActions.RestoreNoteFromArchiveSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.TrashNoteAction action, IDispatcher dispatcher)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var noteFresh = await db.Notes.FindRequiredAsync(action.NoteId);
        noteFresh.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        dispatcher.Dispatch(new NoteActions.TrashNoteSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.RestoreNoteFromTrashAction action, IDispatcher dispatcher)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var noteFresh = await db.Notes.FindRequiredAsync(action.NoteId);
        noteFresh.DeletedAt = null;
        await db.SaveChangesAsync();

        dispatcher.Dispatch(new NoteActions.RestoreNoteFromTrashSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.DeleteNotePermanentlyAction action, IDispatcher dispatcher)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var noteFresh = await db.Notes.FindRequiredAsync(action.NoteId);
        db.Remove(noteFresh);
        await db.SaveChangesAsync();

        dispatcher.Dispatch(new NoteActions.DeleteNotePermanentlySuccessAction(action.NoteId));
    }

    private void Clean(Note note)
    {
        note.Text = note.Text.Trim();
    }
}