namespace BlazingNotes.Logic.Tests.TestSupport;

public static class AppStateExtensions
{
    public static Note GetNote(this AppState state, Guid noteId)
    {
        return state.Notes.Single(n => n.Id == noteId);
    }

    public static Note GetNote(this IState<AppState> state, Guid noteId)
    {
        return state.Value.GetNote(noteId);
    }
}