namespace BlazingNotes.Logic.Services;

public interface INoteStore
{
    Task<List<Note>> GetAllNotesAsync();
    Task<Note> GetByIdAsync(Guid noteId);
    Task AddNoteAsync(Note note);
    Task SaveNoteAsync(Note note);
    Task RemoveAsync(Note noteToRemove);
    Task RemoveRangeAsync(List<Note> notesToRemove);
}