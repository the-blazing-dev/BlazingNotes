namespace BlazingNotes.Logic.Tests.State;

public class AppStateTestBase : TestBase, IAsyncLifetime
{
    public AppStateTestBase(ITestOutputHelper helper) : base(helper)
    {
        Sut = Services.GetRequiredService<IState<AppState>>();
    }

    protected IState<AppState> Sut { get; }

    Task IAsyncLifetime.InitializeAsync()
    {
        return Task.CompletedTask;
    }

    private async Task CheckDbPersistence()
    {
        var dbNotes = await ExecuteOnDb(x => x.Notes.OrderBy(x => x.Id).ToListAsync());
        var stateNotes = Sut.Value.Notes.OrderBy(x => x.Id).ToList();

        dbNotes.Should().BeEquivalentTo(stateNotes);
    }

    public Task DisposeAsync()
    {
        return CheckDbPersistence();
    }
}