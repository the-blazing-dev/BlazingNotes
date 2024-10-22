using System.Text.RegularExpressions;

namespace BlazingNotes.Logic;

public static partial class Constants
{
    // start with word character (including digits and underscores), then also hyphens are allowed
    public static readonly Regex TagRegex = MyRegex();

    [GeneratedRegex(@"(?<![\w-&])#(?:\w[\w-]+)")]
    private static partial Regex MyRegex();
}