using Microsoft.EntityFrameworkCore;

namespace BlazingNotes.Infrastructure;

public static class Extensions
{
    public static async Task<T> FindRequiredAsync<T>(this DbSet<T> dbSet, params object?[]? keyValues) where T : class
    {
        var result = await dbSet.FindAsync(keyValues);
        if (result == null)
        {
            throw new EntityNotFoundException(keyValues);
        }

        return result;
    }
}