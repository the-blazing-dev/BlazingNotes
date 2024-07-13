namespace BlazingNotes.Logic.State;

public static class NoteReducer
{
    [ReducerMethod]
    public static AppState ReduceIncrementCounterAction(AppState state, NoteActions.CreateNoteAction action)
    {
        if (!action.Text.HasContent())
        {
            return state;
        }
        
        var note = new Note
        {
            Text = action.Text.Trim()
        };

        return state with
        {
            Notes = state.Notes.Add(note)
        };
    }
}