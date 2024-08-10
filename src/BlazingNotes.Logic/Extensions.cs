namespace BlazingNotes.Logic;

public static class Extensions
{
    public static bool LacksContent(this string? input)
    {
        return !input.HasContent();
    }

    public static bool LacksContent<T>(this IEnumerable<T>? input)
    {
        return !input.HasContent();
    }
}