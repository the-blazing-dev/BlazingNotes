using BlazingNotes.Infrastructure.Data;
using BlazingNotes.Logic.Entities;
using BlazingNotes.Logic.State;
using Fluxor;
using Microsoft.EntityFrameworkCore;

namespace BlazingNotes.Infrastructure.State;

public class NoteEffects(IDbContextFactory<AppDb> dbFactory)
{
    [EffectMethod]
    public async Task HandleCreateNoteRequestAction(NoteActions.CreateNoteRequestAction action, IDispatcher dispatcher)
    {
        if (!action.Text.HasContent()) return;

        var note = new Note
        {
            Text = action.Text.Trim()
        };

        await using var db = await dbFactory.CreateDbContextAsync();
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        dispatcher.Dispatch(new NoteActions.NoteCreatedAction(note));
    }
}