namespace BlazingNotes.Logic.State;

// let's try to use nested actions
public static class NoteActions
{
    public record CreateNoteAction(string Text);
}