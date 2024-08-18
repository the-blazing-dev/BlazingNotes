using System.Reflection;
using BlazingNotes.Infrastructure.Data;
using BlazingNotes.Infrastructure.State;
using BlazingNotes.Logic.State;
using BlazingNotes.UI.AppFrame;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// todo needed?
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices();
builder.Services.AddFluxor(x => x.ScanAssemblies(Assembly.GetExecutingAssembly(),
    typeof(AppState).Assembly, typeof(NoteEffects).Assembly));

builder.Services.AddDbContextFactory<AppDb>(x => x.UseSqlite("Data Source=appdb.sqlite"));

await builder.Build().RunAsync();