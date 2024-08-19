using BlazingNotes.Infrastructure.Data;
using BlazingNotes.Logic.Entities;
using BlazingNotes.Logic.Services;
using Microsoft.EntityFrameworkCore;

namespace BlazingNotes.Infrastructure.Services;

public class AppDbNoteStore(IDbContextFactory<AppDb> dbContextFactory) : INoteStore
{
    public async Task<List<Note>> GetAllNotesAsync()
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.Notes.AsNoTracking().ToListAsync();
    }

    public async Task<Note> GetByIdAsync(Guid noteId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.Notes.FindRequiredAsync(noteId);
    }

    public async Task AddNoteAsync(Note note)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.Notes.Add(note);
        await db.SaveChangesAsync();
    }

    public async Task SaveNoteAsync(Note note)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        var noteFresh = await db.Notes.FindRequiredAsync(note.Id);
        noteFresh.Text = note.Text;
        noteFresh.ModifiedAt = note.ModifiedAt;
        noteFresh.RelevantAt = note.RelevantAt;
        noteFresh.ArchivedAt = note.ArchivedAt;
        noteFresh.DeletedAt = note.DeletedAt;
        await db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Note noteToRemove)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.Notes.Remove(noteToRemove);
        await db.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(List<Note> notesToRemove)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.Notes.RemoveRange(notesToRemove);
        await db.SaveChangesAsync();
    }
}