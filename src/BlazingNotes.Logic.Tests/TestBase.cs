using BlazingNotes.Logic.State;
using Microsoft.Extensions.DependencyInjection;

namespace BlazingNotes.Logic.Tests;

public class TestBase
{
    private readonly IDispatcher _dispatcher;

    protected TestBase()
    {
        var services = new ServiceCollection();
        services.AddFluxor(options => options.ScanAssemblies(typeof(AppState).Assembly));

        Services = services.BuildServiceProvider();
        var store = Services.GetRequiredService<IStore>();
        _dispatcher = Services.GetRequiredService<IDispatcher>();

        store.InitializeAsync().Wait();
    }

    protected IServiceProvider Services { get; }

    protected void Dispatch(object action)
    {
        _dispatcher.Dispatch(action);
    }
}