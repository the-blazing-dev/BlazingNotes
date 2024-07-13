using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazingNotes.Components;

public class AppComponentBase : FluxorComponent
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    protected void Dispatch(object action)
    {
        Dispatcher.Dispatch(action);
    }
}