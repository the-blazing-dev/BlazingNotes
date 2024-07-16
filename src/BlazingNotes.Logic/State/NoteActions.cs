namespace BlazingNotes.Logic.State;

// let's try to use nested actions
public static class NoteActions
{
    public record CreateNoteRequestAction(string Text);

    public record NoteCreatedAction(Note Note);
}