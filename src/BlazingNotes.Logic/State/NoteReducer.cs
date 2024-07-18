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
            Notes = state.Notes.Add(action.Note)
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
        var oldNote = state.Notes.First(x => x.Id == action.Note.Id);

        return state with
        {
            Notes = state.Notes.Replace(oldNote, action.Note),
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
}