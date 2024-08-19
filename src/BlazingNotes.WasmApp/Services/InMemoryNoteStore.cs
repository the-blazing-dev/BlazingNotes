using BlazingNotes.Logic.Entities;
using BlazingNotes.Logic.Services;

namespace BlazingNotes.WasmApp.Services;

public class InMemoryNoteStore : INoteStore
{
    private readonly Dictionary<Guid, Note> _notes = new();

    public Task<List<Note>> GetAllNotesAsync()
    {
        return Task.FromResult(_notes.Values.ToList());
    }

    public Task<Note> GetByIdAsync(Guid noteId)
    {
        return Task.FromResult(_notes[noteId]);
    }

    public Task AddNoteAsync(Note note)
    {
        _notes.Add(note.Id, note);
        return Task.CompletedTask;
    }

    public Task SaveNoteAsync(Note note)
    {
        _notes[note.Id] = note;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Note noteToRemove)
    {
        _notes.Remove(noteToRemove.Id);
        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(List<Note> notesToRemove)
    {
        foreach (var note in notesToRemove) _notes.Remove(note.Id);
        return Task.CompletedTask;
    }
}