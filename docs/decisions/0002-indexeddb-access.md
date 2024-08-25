---
# These are optional elements. Feel free to remove any of them.
status: accepted
date: 2024-08-23
decision-makers: David Ritter
---
# IndexDb Access

## Context and Problem Statement
In ADR [0001-wasm-app-database.md](0001-wasm-app-database.md) we decided to use IndexedDb as storage technology.

Now the question raises: Who do we invoke the required JS methods?

## Considered Options

* Implement JS Interop on my own
* Use NuGet 

## Decision Outcome

Chosen option: "Use NuGet", because setting it up was very easy and it has everything we need at the moment.

## Pros and Cons of the Options

### Implement JS Interop on my own

[Mozilla Developer Network](https://developer.mozilla.org/en-US/docs/Web/API/IndexedDB_API)

There are quite a few methods to call:
* db opening
* transactions
* object stores

These methods use the callback pattern which does not make things easier. Because of that there are libraries like [idb](https://github.com/jakearchibald/idb) to have JS promises.

* Good, because no dependency
* Good, because high performance possible
* Neutral, because maybe the `idb` dependency will be used
* Bad, because quite a lot of work

### Use NuGet

There are quite a few NuGets out there, I decided to have a closer look at the following two:

* [TG.Blazor.IndexedDB](https://www.nuget.org/packages/TG.Blazor.IndexedDB/)
* [Johnjalani.Blazor.IndexedDB.WebAssembly](https://www.nuget.org/packages/Johnjalani.Blazor.IndexedDB.WebAssembly) (there are quite similar ones around)

The first one is a more low-level abstraction that makes use of the [idb](https://github.com/jakearchibald/idb) JS library.  
It's current version is marked as preview, but it looks quite stable.

The second one is a high-level abstraction that mimics some EF-like behavior and depends on the first one.

First investigations showed that the second NuGet pulls all entries into memory on initialization.  
At some point this could be problematic for us, but we can change this ADR's decision quite easily.

* Good, because everything was up and running very fast
* Good, because technical details have been abstracted
* Neutral, because all data is pulled into memory (done by the 2nd NuGet)
* Bad, because dependencies
* Bad, because the second one has a ChangeDetector bug (fixed)

## More Information

https://github.com/mdn/dom-examples/blob/main/to-do-notifications/scripts/todo.js