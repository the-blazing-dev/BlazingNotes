using System.Reflection;
using BlazingNotes.Logic.Services;
using BlazingNotes.Logic.State;
using BlazingNotes.UI.AppFrame;
using BlazingNotes.WasmApp.Services;
using Blazor.IndexedDB.WebAssembly;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddMudServices();
builder.Services.AddFluxor(x => x.ScanAssemblies(Assembly.GetExecutingAssembly(),
    typeof(AppState).Assembly, typeof(NoteEffects).Assembly));
builder.Services.AddTransient<LoggingJsRuntime>();

builder.Services.AddScoped<IIndexedDbFactory, IndexedDbFactory>(sp =>
{
    var jsRuntime = sp.GetRequiredService<LoggingJsRuntime>();
    var factory = new IndexedDbFactory(jsRuntime);
    return factory;
});
builder.Services.AddScoped<INoteStore, IndexedDbNoteStore>();

await builder.Build().RunAsync();