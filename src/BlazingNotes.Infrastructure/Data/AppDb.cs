using BlazingNotes.Logic.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazingNotes.Infrastructure.Data;

public class AppDb(DbContextOptions<AppDb> options) : DbContext(options)
{
    public DbSet<Note> Notes { get; set; }
}