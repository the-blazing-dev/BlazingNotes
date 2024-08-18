using System.Reflection;
using BlazingNotes.Infrastructure.Data;
using BlazingNotes.Infrastructure.Hosting;
using BlazingNotes.Infrastructure.State;
using BlazingNotes.Infrastructure.Utils;
using BlazingNotes.Logic.State;
using BlazingNotes.UI.AppFrame;
using Fluxor;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddFluxor(x => x.ScanAssemblies(Assembly.GetExecutingAssembly(),
    typeof(AppState).Assembly, typeof(NoteEffects).Assembly));

var connectionString = builder.Configuration.GetConnectionString("AppDb");
connectionString = SqLiteFileHelper.ResolveAndPrepareDatabaseFile(connectionString!);
builder.Services.AddDbContextFactory<AppDb>(x => x.UseSqlite(connectionString));

var app = builder.Build();
await app.Services.MigrateDbAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();