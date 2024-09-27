namespace BlazingNotes.Logic.State;

// @formatter:blank_lines_around_single_line_type 0
// let's try to use nested actions
public static class NoteActions
{
    public record NotesLoadedAction(List<Note> Notes);

    public record CreateNoteRequestAction(string Text, bool IsArchived = false);
    public record NoteCreatedAction(Note Note);
    public record ShowCreateNoteDialogAction;
    public record HideCreateNoteDialogAction;

    public record StartNoteEditingAction(Note Note);
    public record SaveNoteEditingAction(Note Note, string NewText, DateTime? CreatedAt = null);
    public record SaveNoteEditingSuccessAction(Note Note);
    public record CancelNoteEditingAction(Note Note);


    public record ArchiveNoteAction(Guid NoteId);
    public record ArchiveNoteSuccessAction(Note Note);
    public record RestoreNoteFromArchiveAction(Guid NoteId);
    public record RestoreNoteFromArchiveSuccessAction(Note Note);

    public record TrashNoteAction(Guid NoteId);
    public record TrashNoteSuccessAction(Note Note);
    public record RestoreNoteFromTrashAction(Guid NoteId);
    public record RestoreNoteFromTrashSuccessAction(Note Note);

    public record DeleteNotePermanentlyAction(Guid NoteId);
    public record DeleteNotePermanentlySuccessAction(Guid NoteId);
}