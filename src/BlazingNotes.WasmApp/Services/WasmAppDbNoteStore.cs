using BlazingNotes.Logic.Entities;
using BlazingNotes.Logic.Services;
using Blazor.IndexedDB.WebAssembly;

namespace BlazingNotes.WasmApp.Services;

public class WasmAppDbNoteStore(IIndexedDbFactory dbFactory) : INoteStore
{
    private WasmAppDb _db = null!;

    public async Task<List<Note>> GetAllNotesAsync()
    {
        _db = await dbFactory.Create<WasmAppDb>("BlazingNotesDb");
        return _db.Notes.ToList();
    }

    public Task<Note> GetByIdAsync(Guid noteId)
    {
        return Task.FromResult(_db.Notes.Single(x => x.Id == noteId));
    }

    public async Task AddNoteAsync(Note note)
    {
        _db.Notes.Add(note);
        await _db.SaveChanges();
    }

    public async Task SaveNoteAsync(Note note)
    {
        // internal change detection handles everything needed
        await _db.SaveChanges();
    }

    public async Task RemoveAsync(Note noteToRemove)
    {
        _db.Notes.Remove(noteToRemove);
        await _db.SaveChanges();
    }

    public async Task RemoveRangeAsync(List<Note> notesToRemove)
    {
        notesToRemove.ForEach(x => _db.Notes.Remove(x));
        await _db.SaveChanges();
    }
}