namespace BlazingNotes.Logic;

public record ExportModel
{
    public int Version { get; init; } = 1;
    public DateTime ExportedAt { get; init; }
    public required List<Note> Notes { get; init; }
}