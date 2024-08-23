using BlazingNotes.Logic.Entities;
using Blazor.IndexedDB.WebAssembly;
using Microsoft.JSInterop;

namespace BlazingNotes.WasmApp.Services;

public class WasmAppDb : IndexedDb
{
    public WasmAppDb(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version)
    {
    }

    public IndexedSet<Note> Notes { get; set; }
}