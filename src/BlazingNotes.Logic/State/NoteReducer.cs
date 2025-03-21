namespace BlazingNotes.Logic.State;

public static class NoteReducer
{
    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.NotesLoadedAction action)
    {
        return state with
        {
            Notes = action.Notes.ToImmutableList()
        };
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.NoteCreatedAction action)
    {
        return state with
        {
            Notes = state.Notes.Add(action.Note),
            ShowCreateNoteDialog = false // for now just always close the possible dialog
        };
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.StartNoteEditingAction action)
    {
        return state with
        {
            CurrentlyEditingNote = action.Note
        };
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.SaveNoteEditingSuccessAction action)
    {
        var newState = UpdateNote(state, action.Note);
        return newState with
        {
            CurrentlyEditingNote = null
        };
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.CancelNoteEditingAction action)
    {
        // for now assume that the "correct" not is canceled
        return state with
        {
            CurrentlyEditingNote = null
        };
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.ShowCreateNoteDialogAction action)
    {
        return state with
        {
            ShowCreateNoteDialog = true
        };
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.HideCreateNoteDialogAction action)
    {
        return state with
        {
            ShowCreateNoteDialog = false
        };
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.ArchiveNoteSuccessAction action)
    {
        return UpdateNote(state, action.Note);
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.RestoreNoteFromArchiveSuccessAction action)
    {
        return UpdateNote(state, action.Note);
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.TrashNoteSuccessAction action)
    {
        return UpdateNote(state, action.Note);
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.RestoreNoteFromTrashSuccessAction action)
    {
        return UpdateNote(state, action.Note);
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.DeleteNotePermanentlySuccessAction action)
    {
        var needsToClearEditingNote = state.CurrentlyEditingNote?.Id == action.NoteId;
        return state with
        {
            Notes = state.Notes.RemoveAll(x => x.Id == action.NoteId),
            CurrentlyEditingNote = needsToClearEditingNote ? null : state.CurrentlyEditingNote
        };
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.HideForSuccessAction action)
    {
        return UpdateNote(state, action.Note);
    }

    [ReducerMethod]
    public static AppState Reduce(AppState state, NoteActions.UnhideSuccessAction action)
    {
        return UpdateNote(state, action.Note);
    }

    private static AppState UpdateNote(AppState state, Note updatedNote)
    {
        var oldNote = state.Notes.First(x => x.Id == updatedNote.Id);

        return state with
        {
            Notes = state.Notes.Replace(oldNote, updatedNote)
        };
    }
}