namespace BlazingNotes.Logic.State;

public static class NoteReducer
{
    [ReducerMethod]
    public static AppState ReduceNotesLoadedAction(AppState state, NoteActions.NotesLoadedAction action)
    {
        return state with
        {
            Notes = action.Notes.ToImmutableList()
        };
    }
    
    [ReducerMethod]
    public static AppState ReduceNoteCreatedAction(AppState state, NoteActions.NoteCreatedAction action)
    {
        return state with
        {
            Notes = state.Notes.Add(action.Note)
        };
    }
}