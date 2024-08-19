using System.Reflection;
using BlazingNotes.Logic.State;
using BlazingNotes.UI.AppFrame;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// todo needed?
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices();
builder.Services.AddFluxor(x => x.ScanAssemblies(Assembly.GetExecutingAssembly(),
    typeof(AppState).Assembly, typeof(NoteEffects).Assembly));

await builder.Build().RunAsync();