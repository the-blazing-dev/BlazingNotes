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

    private void Clean(Note note)
    {
        note.Text = note.Text.Trim();
    }
}