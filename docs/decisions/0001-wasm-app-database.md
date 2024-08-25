---
status: accepted
date: 2024-08-18
decision-makers: David Ritter
---
# Wasm-App Database

## Context and Problem Statement

The Wasm-App also needs some kind of persistence / database.

## Considered Options

* SQLite inside the browser
* use browser's IndexedDb

## Decision Outcome

Chosen option: "use browser's IndexedDb", because it feels more like the standard conform way.  
Also I didn't get the "SQLite inside the browser" running.

## Pros and Cons of the Options

### SQLite inside the browser

Beginning with .NET 6 there is the possibility to include native dependencies, SQLite in our case.
Proof of concepts are around, but there isn't that much out there.

Some links:
- [First concept video from Steve Sanderson](https://www.youtube.com/watch?v=kesUNeBZ1Os)
- [Code from Steve Sanderson](https://github.com/SteveSandersonMS/BlazeOrbital/blob/6b5f7892afbdc96871c974eb2d30454df4febb2c/BlazeOrbital/ManufacturingHub/BlazeOrbital.ManufacturingHub.csproj)
- [Issue regarding the upper two](https://github.com/dotnet/aspnetcore/issues/39825)
- [fingers10/BlazorWasmEFCore on GitHub](https://github.com/fingers10/BlazorWasmEFCore/tree/master)
- [SqLiteWasmHelper](https://github.com/JeremyLikness/SqliteWasmHelper)
- [Notion's journey](https://www.notion.so/blog/how-we-sped-up-notion-in-the-browser-with-wasm-sqlite)

* Good, because we could use SQLite both in ServerApp and WasmApp
* Good, because we don't need a Store abstraction / Repository pattern
* Neutral, because some funky csproj settings are needed
* Neutral, because it seems like some niche work
* Bad, because some "SQLite in-process to something persisting" is still needed
* Bad, because there can be multi-tab-problems (see Notion's journey)
* Bad, because I didn't get it running

### use browser's IndexedDb

IndexedDb is a native database available in all major browsers. You can save document (kinda JSON objects).

Some links:
- [IndexedDb at MDN docs](https://developer.mozilla.org/en-US/docs/Web/API/IndexedDB_API)
- [caniuse](https://caniuse.com/?search=indexeddb#google_vignette)
- [Nuget: Johnjalani.Blazor.IndexedDB.WebAssembly](https://github.com/johnjalani/Blazor.IndexedDB.WebAssembly)

* Good, because natively supported and widely available
* Good, because there are more NuGets and tutorials out there than for SQLite
* Good, because something new interesting to learn
* Good, because storage performance should be better
* Neutral, because store abstraction is needed (which could also result in better code and testability)
* Bad, because {argument d}

## More Information

### SQLite inside the browser installation journey

With the following csproj setup:

```xml
<PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <EmccExtraLDFlags>-s WARN_ON_UNDEFINED_SYMBOLS=0</EmccExtraLDFlags>
</PropertyGroup>

<ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="8.0.5"/>
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="8.0.5" PrivateAssets="all"/>
    <PackageReference Include="SQLitePCLRaw.bundle_e_sqlite3" Version="2.1.9" />
</ItemGroup>
```

I get the build warning:
```
C:\Program Files\dotnet\sdk-manifests\8.0.100\microsoft.net.workload.mono.toolchain.current\8.0.5\WorkloadManifest.targets(186,5): warning : @(NativeFil
eReference) is not empty, but the native references won't be linked in, because neither $(WasmBuildNative), nor $(RunAOTCompilation) are 'true'. NativeF
ileReference=C:\Users\david\.nuget\packages\sqlitepclraw.lib.e_sqlite3\2.1.9\buildTransitive\net8.0\..\..\runtimes\browser-wasm\nativeassets\net8.0\e_sq 
lite3.a [E:\bzdev\BlazingNotes\src\BlazingNotes.WasmApp\BlazingNotes.WasmApp.csproj]
```

When adding this to the csproj:
```xml
<PropertyGroup>
    <WasmBuildNative>true</WasmBuildNative>
</PropertyGroup>
```

I get the following build error (in German):
```
0>Microsoft.NET.Sdk.ImportWorkloads.targets(38,5): Error NETSDK1147 : Zum Erstellen dieses Projekts müssen die folgenden Workloads installiert sein: wasm-tools
Führen Sie den folgenden Befehl aus, um diese Workloads zu installieren: dotnet workload restore
```

After executing `dotnet workload restore` a thousand things have been installed:
```
Workloads werden installiert: wasm-tools

Für den Computer steht ein Neustart aus. Der Workloadvorgang wird fortgesetzt, aber Sie müssen möglicherweise neu starten.
Downloading microsoft.net.sdk.android.manifest-8.0.100.msi.x64 (34.0.113)
microsoft.net.sdk.android.manifest-8.0.100.msi.x64 wird installiert ..... Done
Downloading microsoft.net.sdk.ios.manifest-8.0.100.msi.x64 (17.5.8020)
microsoft.net.sdk.ios.manifest-8.0.100.msi.x64 wird installiert .... Done
Downloading microsoft.net.sdk.maccatalyst.manifest-8.0.100.msi.x64 (17.5.8020)
microsoft.net.sdk.maccatalyst.manifest-8.0.100.msi.x64 wird installiert ..... Done
Downloading microsoft.net.sdk.macos.manifest-8.0.100.msi.x64 (14.5.8020)
microsoft.net.sdk.macos.manifest-8.0.100.msi.x64 wird installiert .... Done
Downloading microsoft.net.sdk.maui.manifest-8.0.100.msi.x64 (8.0.72)
microsoft.net.sdk.maui.manifest-8.0.100.msi.x64 wird installiert .... Done
Downloading microsoft.net.sdk.tvos.manifest-8.0.100.msi.x64 (17.5.8020)
microsoft.net.sdk.tvos.manifest-8.0.100.msi.x64 wird installiert .... Done
Downloading microsoft.net.workload.mono.toolchain.current.manifest-8.0.100.msi.x64 (8.0.8)
microsoft.net.workload.mono.toolchain.current.manifest-8.0.100.msi.x64 wird installiert ..... Done
Downloading microsoft.net.workload.emscripten.current.manifest-8.0.100.msi.x64 (8.0.8)
microsoft.net.workload.emscripten.current.manifest-8.0.100.msi.x64 wird installiert .... Done
Downloading microsoft.net.workload.emscripten.net6.manifest-8.0.100.msi.x64 (8.0.8)
microsoft.net.workload.emscripten.net6.manifest-8.0.100.msi.x64 wird installiert .... Done
Downloading microsoft.net.workload.emscripten.net7.manifest-8.0.100.msi.x64 (8.0.8)
microsoft.net.workload.emscripten.net7.manifest-8.0.100.msi.x64 wird installiert .... Done
Downloading microsoft.net.workload.mono.toolchain.net6.manifest-8.0.100.msi.x64 (8.0.8)
microsoft.net.workload.mono.toolchain.net6.manifest-8.0.100.msi.x64 wird installiert ..... Done
Downloading microsoft.net.workload.mono.toolchain.net7.manifest-8.0.100.msi.x64 (8.0.8)
microsoft.net.workload.mono.toolchain.net7.manifest-8.0.100.msi.x64 wird installiert .... Done
Downloading microsoft.net.sdk.aspire.manifest-8.0.100.msi.x64 (8.1.0)
microsoft.net.sdk.aspire.manifest-8.0.100.msi.x64 wird installiert .... Done
Downloading Microsoft.NET.Runtime.WebAssembly.Sdk.Msi.x64 (8.0.8)
Microsoft.NET.Runtime.WebAssembly.Sdk.Msi.x64 wird installiert ..... Done
Downloading Microsoft.NETCore.App.Runtime.Mono.browser-wasm.Msi.x64 (8.0.8)
Microsoft.NETCore.App.Runtime.Mono.browser-wasm.Msi.x64 wird installiert ........ Done
Downloading Microsoft.NETCore.App.Runtime.AOT.win-x64.Cross.browser-wasm.Msi.x64 (8.0.8)
Microsoft.NETCore.App.Runtime.AOT.win-x64.Cross.browser-wasm.Msi.x64 wird installiert ..... Done
Downloading Microsoft.NET.Runtime.MonoAOTCompiler.Task.Msi.x64 (8.0.8)
Microsoft.NET.Runtime.MonoAOTCompiler.Task.Msi.x64 wird installiert ..... Done
Downloading Microsoft.NET.Runtime.MonoTargets.Sdk.Msi.x64 (8.0.8)
Microsoft.NET.Runtime.MonoTargets.Sdk.Msi.x64 wird installiert ..... Done
Downloading Microsoft.NET.Runtime.Emscripten.3.1.34.Node.win-x64.Msi.x64 (8.0.8)
Microsoft.NET.Runtime.Emscripten.3.1.34.Node.win-x64.Msi.x64 wird installiert ..... Done
Downloading Microsoft.NET.Runtime.Emscripten.3.1.34.Python.win-x64.Msi.x64 (8.0.8)
Microsoft.NET.Runtime.Emscripten.3.1.34.Python.win-x64.Msi.x64 wird installiert ............... Done
Downloading Microsoft.NET.Runtime.Emscripten.3.1.34.Cache.win-x64.Msi.x64 (8.0.8)
Microsoft.NET.Runtime.Emscripten.3.1.34.Cache.win-x64.Msi.x64 wird installiert ........................ Done
Downloading Microsoft.NET.Runtime.Emscripten.3.1.34.Sdk.win-x64.Msi.x64 (8.0.8)
Microsoft.NET.Runtime.Emscripten.3.1.34.Sdk.win-x64.Msi.x64 wird installiert .............................................................. Done

Workload(s) wasm-tools wurde(n) erfolgreich installiert.
```

I did some more trying with [fingers10/BlazorWasmEFCore on GitHub](https://github.com/fingers10/BlazorWasmEFCore/tree/master) where I also got it running inside the Home.razor page.
But when trying to register it as a service, nothing worked any more (I even got service lookup exceptions).

Then I canceled.
