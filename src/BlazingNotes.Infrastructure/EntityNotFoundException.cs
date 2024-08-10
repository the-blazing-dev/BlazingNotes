namespace BlazingNotes.Infrastructure;

public class EntityNotFoundException(object?[]? keyValues) : Exception
{
    public object?[]? KeyValues { get; } = keyValues;
}