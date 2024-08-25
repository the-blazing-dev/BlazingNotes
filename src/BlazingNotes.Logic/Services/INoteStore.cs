namespace BlazingNotes.Logic.Services;

public interface INoteStore
{
    /// <summary>
    /// The name to show on the InfoPage
    /// </summary>
    string GetName();

    /// <summary>
    /// The description to show on the InfoPage
    /// </summary>
    string GetDescription();

    Task<List<Note>> GetAllNotesAsync();
    Task<Note> GetByIdAsync(Guid noteId);
    Task AddNoteAsync(Note note);
    Task SaveNoteAsync(Note note);
    Task RemoveAsync(Note noteToRemove);
    Task RemoveRangeAsync(List<Note> notesToRemove);
}