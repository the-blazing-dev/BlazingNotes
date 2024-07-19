using BlazingNotes.Infrastructure.Data;
using BlazingNotes.Logic.Entities;
using BlazingNotes.Logic.State;
using Fluxor;
using Microsoft.EntityFrameworkCore;

namespace BlazingNotes.Infrastructure.State;

public class NoteEffects(IDbContextFactory<AppDb> dbFactory)
{
    [EffectMethod]
    public async Task HandleStoreInitializedAction(StoreInitializedAction action, IDispatcher dispatcher)
    {
        // this is a very pragmatic approach: as the intention is to only store very small notes
        // and this app works with a local database, we can just load all notes into memory
        // which gives us advantages like instant search and faster UI experience
        // as soon as this will lead to too large memory consumption we have to rethink the approach

        await using var db = await dbFactory.CreateDbContextAsync();
        var notes = await db.Notes.AsNoTracking().ToListAsync();
        dispatcher.Dispatch(new NoteActions.NotesLoadedAction(notes));
    }

    [EffectMethod]
    public async Task HandleCreateNoteRequestAction(NoteActions.CreateNoteRequestAction action, IDispatcher dispatcher)
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
    public async Task HandleSaveNoteEditAction(NoteActions.SaveNoteEditingAction action, IDispatcher dispatcher)
    {
        if (!action.NewText.HasContent())
        {
            // ignored for now
            dispatcher.Dispatch(new NoteActions.CancelNoteEditingAction(action.Note));
            return;
        }

        // possible improvement for somewhen: check if there is a "real" change

        await using var db = await dbFactory.CreateDbContextAsync();
        var noteFresh = await db.Notes.FindAsync(action.Note.Id); // todo FindRequiredAsync
        noteFresh.Text = action.NewText;
        Clean(noteFresh);
        await db.SaveChangesAsync();

        dispatcher.Dispatch(new NoteActions.SaveNoteEditingSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.ArchiveNoteAction action, IDispatcher dispatcher)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var noteFresh = await db.Notes.FindAsync(action.NoteId); // todo FindRequiredAsync
        noteFresh.IsArchived = true;
        await db.SaveChangesAsync();

        dispatcher.Dispatch(new NoteActions.ArchiveNoteSuccessAction(noteFresh));
    }

    private void Clean(Note note)
    {
        note.Text = note.Text.Trim();
    }
}