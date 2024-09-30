using Microsoft.AspNetCore.Components.Web;

namespace BlazingNotes.UI.Extensions;

public static class KeyboardEventExtensions
{
    public static bool IsSaveShortcut(this KeyboardEventArgs args, out bool isArchived)
    {
        if (args.CtrlKey && args.Key == "Enter")
        {
            isArchived = args.AltKey;
            return true;
        }

        isArchived = false;
        return false;
    }
}