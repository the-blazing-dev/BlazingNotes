using System.Text.Json;
using System.Text.Json.Serialization;
using BlazingNotes.Logic.Services;
using Microsoft.Extensions.Logging;

namespace BlazingNotes.Logic.State;

public class NoteEffects(INoteStore noteStore, IDownloadFileService downloadFileService, ILogger<NoteEffects> logger)
{
    [EffectMethod]
    public async Task Handle(StoreInitializedAction action, IDispatcher dispatcher)
    {
        // this is a very pragmatic approach: as the intention is to only store very small notes
        // and this app works with a local database, we can just load all notes into memory
        // which gives us advantages like instant search and faster UI experience
        // as soon as this will lead to too large memory consumption we have to rethink the approach

        var notes = await noteStore.GetAllNotesAsync();
        notes = await DeleteOldTrashedNotes(notes);

        dispatcher.Dispatch(new NoteActions.NotesLoadedAction(notes));
    }

    private async Task<List<Note>> DeleteOldTrashedNotes(List<Note> notes)
    {
        var trashClearLimit = DateTime.UtcNow.Date.AddDays(-30);
        var notesToRemove = notes.Where(n => n.DeletedAt <= trashClearLimit).ToList();
        if (notesToRemove.Any())
        {
            await noteStore.RemoveRangeAsync(notesToRemove);
            logger.LogInformation("Finally deleted {Count} notes from db", notesToRemove.Count);
            notes = notes.Except(notesToRemove).ToList();
        }

        return notes;
    }

    [EffectMethod]
    public async Task Handle(NoteActions.CreateNoteRequestAction action, IDispatcher dispatcher)
    {
        if (!action.Text.HasContent())
        {
            return;
        }

        var note = new Note
        {
            Text = action.Text.Trim(),
            ArchivedAt = action.IsArchived ? DateTime.UtcNow : null
        };

        Clean(note);
        await noteStore.AddNoteAsync(note);

        dispatcher.Dispatch(new NoteActions.NoteCreatedAction(note));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.SaveNoteEditingAction action, IDispatcher dispatcher)
    {
        if (!action.NewText.HasContent() &&
            action.RelevantAt == null)
        {
            // ignored for now
            dispatcher.Dispatch(new NoteActions.CancelNoteEditingAction(action.Note));
            return;
        }

        var noteFresh = await noteStore.GetByIdAsync(action.Note.Id);
        var oldText = noteFresh.Text;
        noteFresh.Text = action.NewText;
        Clean(noteFresh);

        // only update entity + timestamp on "real" changes
        if (noteFresh.Text != oldText)
        {
            noteFresh.ModifiedAt = DateTime.UtcNow;
        }

        if (action.RelevantAt.HasValue)
        {
            noteFresh.RelevantAt = action.RelevantAt.Value;
        }

        await noteStore.SaveNoteAsync(noteFresh);

        dispatcher.Dispatch(new NoteActions.SaveNoteEditingSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.ArchiveNoteAction action, IDispatcher dispatcher)
    {
        var noteFresh = await noteStore.GetByIdAsync(action.NoteId);
        noteFresh.ArchivedAt = DateTime.UtcNow;
        await noteStore.SaveNoteAsync(noteFresh);

        dispatcher.Dispatch(new NoteActions.ArchiveNoteSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.RestoreNoteFromArchiveAction action, IDispatcher dispatcher)
    {
        var noteFresh = await noteStore.GetByIdAsync(action.NoteId);
        noteFresh.ArchivedAt = null;
        await noteStore.SaveNoteAsync(noteFresh);

        dispatcher.Dispatch(new NoteActions.RestoreNoteFromArchiveSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.TrashNoteAction action, IDispatcher dispatcher)
    {
        var noteFresh = await noteStore.GetByIdAsync(action.NoteId);
        noteFresh.DeletedAt = DateTime.UtcNow;
        await noteStore.SaveNoteAsync(noteFresh);

        dispatcher.Dispatch(new NoteActions.TrashNoteSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.RestoreNoteFromTrashAction action, IDispatcher dispatcher)
    {
        var noteFresh = await noteStore.GetByIdAsync(action.NoteId);
        noteFresh.DeletedAt = null;
        await noteStore.SaveNoteAsync(noteFresh);

        dispatcher.Dispatch(new NoteActions.RestoreNoteFromTrashSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.DeleteNotePermanentlyAction action, IDispatcher dispatcher)
    {
        var noteFresh = await noteStore.GetByIdAsync(action.NoteId);
        await noteStore.RemoveAsync(noteFresh);

        dispatcher.Dispatch(new NoteActions.DeleteNotePermanentlySuccessAction(action.NoteId));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.HideForAction action, IDispatcher dispatcher)
    {
        var noteFresh = await noteStore.GetByIdAsync(action.NoteId);
        // use DateTime.Now (and not UtcNow) because the user wants it to be hidden from his point of view
        noteFresh.HiddenUntil = DateTime.Now.Add(action.Duration);
        await noteStore.SaveNoteAsync(noteFresh);

        dispatcher.Dispatch(new NoteActions.HideForSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.UnhideAction action, IDispatcher dispatcher)
    {
        var noteFresh = await noteStore.GetByIdAsync(action.NoteId);
        noteFresh.HiddenUntil = null;
        await noteStore.SaveNoteAsync(noteFresh);

        dispatcher.Dispatch(new NoteActions.UnhideSuccessAction(noteFresh));
    }

    [EffectMethod]
    public async Task Handle(NoteActions.ExportNotesAction action, IDispatcher dispatcher)
    {
        var notes = await noteStore.GetAllNotesAsync();
        var notDeletedNotes = notes.Where(x => x.DeletedAt == null).ToList();

        var model = new ExportModel()
        {
            Notes = notDeletedNotes,
            ExportedAt = DateTime.UtcNow
        };

        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var bytes = JsonSerializer.SerializeToUtf8Bytes(model, options);
        var stream = new MemoryStream(bytes);

        // use local time here because users will see it
        var fileName = $"bznotes-backup-{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.json";

        await downloadFileService.ProvideFileAsync(stream, fileName);
    }

    private void Clean(Note note)
    {
        note.Text = note.Text.Trim();
    }
}