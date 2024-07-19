namespace BlazingNotes.Logic.State;

// @formatter:blank_lines_around_single_line_type 0
// let's try to use nested actions
public static class NoteActions
{
    public record NotesLoadedAction(List<Note> Notes);

    public record CreateNoteRequestAction(string Text);
    public record NoteCreatedAction(Note Note);
    public record ShowCreateNoteDialogAction;
    public record HideCreateNoteDialogAction;

    public record StartNoteEditingAction(Note Note);
    public record SaveNoteEditingAction(Note Note, string NewText);
    public record SaveNoteEditingSuccessAction(Note Note);
    public record CancelNoteEditingAction(Note Note);


    public record ArchiveNoteAction(Guid NoteId);
    public record ArchiveNoteSuccessAction(Note Note);
    public record RestoreNoteAction(Guid NoteId);
    public record RestoreNoteSuccessAction(Note Note);
}