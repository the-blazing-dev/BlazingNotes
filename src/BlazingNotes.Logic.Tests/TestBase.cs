using BlazingNotes.Infrastructure.Data;
using BlazingNotes.Infrastructure.State;
using BlazingNotes.Logic.State;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using UnhandledExceptionEventArgs = Fluxor.Exceptions.UnhandledExceptionEventArgs;

namespace BlazingNotes.Logic.Tests;

public class TestBase
{
    private readonly IDispatcher _dispatcher;
    private readonly IStore _store;
    private readonly ITestOutputHelper _testOutputHelper;

    protected TestBase(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var services = new ServiceCollection();
        services.AddDbContextFactory<AppDb>(x => x.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddFluxor(options => options.ScanAssemblies(
            typeof(AppState).Assembly,
            typeof(NoteEffects).Assembly // we need to reference the effects because they dispatch further effects
        ));

        Services = services.BuildServiceProvider();
        
        _store = Services.GetRequiredService<IStore>();
        _dispatcher = Services.GetRequiredService<IDispatcher>();

        _store.InitializeAsync().Wait();
        _store.UnhandledException += StoreOnUnhandledException;
    }

    protected IServiceProvider Services { get; }

    private void StoreOnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        _testOutputHelper.WriteLine("Unhandled fluxor exception: " + e.Exception);
    }

    protected void Dispatch(object action)
    {
        _dispatcher.Dispatch(action);
    }
}