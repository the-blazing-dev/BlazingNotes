namespace BlazingNotes.Logic.State;

public static class NoteReducer
{
    [ReducerMethod]
    public static AppState ReduceIncrementCounterAction(AppState state, NoteActions.CreateNoteAction action)
    {
        var note = new Note
        {
            Text = action.Text
        };

        return state with
        {
            Notes = state.Notes.Add(note)
        };
    }
}