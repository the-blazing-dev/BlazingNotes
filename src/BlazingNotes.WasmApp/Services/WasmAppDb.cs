using System.Collections;
using System.Reflection;
using BlazingNotes.Logic.Entities;
using Blazor.IndexedDB.WebAssembly;
using Microsoft.JSInterop;

namespace BlazingNotes.WasmApp.Services;

public class WasmAppDb : IndexedDb
{
    public WasmAppDb(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version)
    {
    }

    public IndexedSet<Note> Notes { get; set; } = null!; // set by IndexedDb

    public new async Task SaveChanges()
    {
        await base.SaveChanges();
        FixChangeDetectorState();
    }

    // the change detection keeps its old state so updates are saved every time - again and again
    // same for adding entries which results in a JS error
    // maybe the general approach is to use short-lived db instances, but then "loading all data by default" is not perfect IMHO.
    private void FixChangeDetectorState()
    {
        var privateMember = BindingFlags.Instance | BindingFlags.NonPublic;
        var assembly = typeof(IndexedDb).Assembly;
        var indexedEntityType = assembly.GetType("Blazor.IndexedDB.WebAssembly.IndexedEntity", true)!;
        var indexedEntityStateProperty = indexedEntityType.GetProperty("State", privateMember)!;

        var internalItemsField = Notes.GetType().GetField("internalItems", privateMember);

        if (internalItemsField == null)
        {
            throw new Exception(
                "Could not get private field 'internalItems' from IndexedSet. Did you update the NuGet?");
        }

        var internalItems = (IEnumerable)internalItemsField.GetValue(Notes)!;
        foreach (var item in internalItems)
        {
            var state = (EntityState)indexedEntityStateProperty.GetValue(item, null)!;
            if (state is EntityState.Added or EntityState.Modified)
            {
                indexedEntityStateProperty.SetValue(item, EntityState.Unchanged);
            }
            else if (state is EntityState.Deleted)
            {
                // to be perfect we would need to delete the item from the internalItems list.
                // as we don't really depend on it to be 100% synced with the browser's indexeddb,
                // it is good enough to just remove this item from further SaveChanges() handling
                indexedEntityStateProperty.SetValue(item, EntityState.Detached);
            }
        }
    }
}