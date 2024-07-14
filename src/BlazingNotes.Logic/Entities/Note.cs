namespace BlazingNotes.Logic.Entities;

public class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Text { get; set; }
}