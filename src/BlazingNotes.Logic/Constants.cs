using System.Text.RegularExpressions;

namespace BlazingNotes.Logic;

public static class Constants
{
    public static readonly Regex TagRegex = new Regex("#[a-zA-Z0-9-_]+");
}