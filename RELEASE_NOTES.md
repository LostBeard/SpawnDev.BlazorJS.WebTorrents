# SpawnDev.BlazorJS.WebTorrents — Release Notes

## v2.0.2

**NuGet:** [`SpawnDev.BlazorJS.WebTorrents 2.0.2`](https://www.nuget.org/packages/SpawnDev.BlazorJS.WebTorrents/2.0.2)  
**Released:** 2025  
**Targets:** .NET 8 · .NET 9 · .NET 10  
**Bundled WebTorrent JS:** 2.8.3  
**Requires:** [SpawnDev.BlazorJS](https://www.nuget.org/packages/SpawnDev.BlazorJS) 3.0.2+

---

### What's New

#### .NET 10 Support

The library now targets `net8.0`, `net9.0`, **and `net10.0`** in a single NuGet package.  
A build-time fix (`StaticWebAssetFingerprintingEnabled=false`) prevents .NET 10's new static-web-asset fingerprinting (cache-busting hashes) from breaking the bundled Service Worker scripts, which must be served at predictable paths.

#### Updated WebTorrent JS Library (2.8.3)

The bundled `webtorrent.min.js` and `sw.min.js` have been updated to **WebTorrent 2.8.3**.  
The current bundled version is exposed at runtime:

```csharp
// The version string of the bundled JS library
string bundledVersion = webTorrentService.BundledLibraryVersion; // "2.8.3"

// The version actually loaded in the browser
string runtimeVersion = WebTorrent.LibraryVersion;
```

You can also load a different version by passing a custom URL to `WebTorrent.ImportWebTorrent`:

```csharp
// Use a specific CDN version instead of the bundled one
await WebTorrent.ImportWebTorrent("https://cdn.jsdelivr.net/npm/webtorrent@2.8.4/dist/webtorrent.min.js");
```

#### Updated Core Dependency: SpawnDev.BlazorJS 3.0.2

This release requires **SpawnDev.BlazorJS 3.0.2** (a major-version bump from the 2.x line used by v1.x).  
The updated base library brings improved JS interop, better `JSObject` lifecycle management, and the new `Union<>` discriminated-union type that is used throughout this library's API surface.

#### Recent-Torrent Persistence

`WebTorrentService` now automatically saves torrent metadata to the browser's **Origin Private File System (OPFS)** when a torrent becomes ready, and reloads those torrents on the next app start. Three new properties control this behaviour:

| Property | Type | Default | Description |
|---|---|---|---|
| `EnableRecent` | `bool` | `true` | Save/reload recent torrents automatically |
| `LoadRecentPaused` | `bool` | `false` | Reload recent torrents in a paused state |
| `LoadRecentDeselected` | `bool` | `false` | Reload recent torrents with no pieces selected |

```csharp
builder.Services.AddWebTorrentService(configureCallback: svc =>
{
    svc.EnableRecent = true;
    svc.LoadRecentPaused = true;       // don't start downloading immediately
    svc.LoadRecentDeselected = false;   // keep previously selected files selected
});
```

#### `Torrent.WhenReady()` Async Helpers

Three new overloads let you `await` a torrent becoming ready without manual event wiring:

```csharp
// Wait indefinitely
await torrent.WhenReady();

// Wait with a CancellationToken
await torrent.WhenReady(cancellationToken);

// Wait with a timeout (throws OperationCanceledException on timeout)
await torrent.WhenReady(timeoutMS: 10_000);
```

These complement the existing event-based `torrent.OnReady` and the synchronous `torrent.Ready` property.

#### Service Worker Server — Simplified Setup

A new `RegisterServerServiceWorker` extension method on `WebTorrent` handles service-worker registration and `CreateServer` in one call, replacing the previous multi-step setup:

```csharp
// In your component or service
await WebTorrentService.Client!.RegisterServerServiceWorker();
// or, to register but not call CreateServer yet:
await WebTorrentService.Client!.RegisterServerServiceWorker(createServer: false);
```

The service worker enables **HTTP range-request streaming** of torrent files directly to `<video>`, `<audio>`, and `<img>` elements via `File.StreamTo()` and `File.StreamURL`.

> **Service Worker shim requirement** – your `wwwroot/service-worker.js` (or equivalent SW entry point) must import the bundled script so it can intercept streaming requests:
>
> ```js
> // wwwroot/service-worker.js
> self.importScripts('sw.min.js');
> ```

#### `WebTorrentService.EnableServer()` Convenience Method

A new awaitable method ensures the Service Worker server is started at most once, regardless of how many components call it:

```csharp
bool started = await webTorrentService.EnableServer();
```

#### Storage Management Helpers

New extension methods on `WebTorrent` give access to the underlying OPFS torrent storage:

```csharp
// List all torrent names that have stored data
List<string> names = await webTorrentService.Client!.GetTorrentStorageNames();

// Delete all stored torrent data
List<string> deleted = await webTorrentService.Client!.ClearTorrentStorage();

// Delete stored data for a specific torrent
bool removed = await webTorrentService.Client!.ClearTorrentStorage("Big Buck Bunny");
```

#### Instance-ID Lookup Helpers

`Torrent` and `Wire` objects are JS-owned and cannot be stored by reference across renders. The new **InstanceId** property (lazily assigned, stored on the JS object itself) and lookup helpers solve this:

```csharp
// Get a stable string key for a torrent
string id = torrent.InstanceId;

// Later, look it up from the client
if (webTorrentService.Client!.GetTorrentByInstanceId(id, out var t))
{
    // t is the Torrent, properly wrapped
}

// Same for wires
if (webTorrentService.Client!.GetWireByInstanceId(wireId, out var torrent2, out var wire))
{
    // torrent2 and wire are live objects
}
```

#### Poster Image Helpers

Two new helpers on `WebTorrentService` find a torrent's "poster" image file (a file named `poster.*`, or the first image file in the torrent) and return it as a base64 data URL:

```csharp
// Returns a base64 data URL if a completed image file is found, or "" otherwise
string posterUrl = await webTorrentService.GetTorrentPoster(torrent);

// Returns the File object (caller owns disposal)
File? posterFile = await webTorrentService.GetTorrentPosterFile(torrent);
```

#### Public Tracker List and Magnet URI Utilities

`WebTorrentService` exposes a curated list of public WebSocket trackers and utility methods:

```csharp
// Known public trackers (wss:// and udp://)
List<string> trackers = WebTorrentService.PublicTrackers;

// Trackers currently configured in the client
string[] announce = webTorrentService.Announce;

// Best trackers to use when seeding (configured, or public if none)
string[] seedTrackers = webTorrentService.SeedTrackers;

// Convert an info hash to a magnet URI
string magnet = WebTorrentService.InfoHashToMagnet(
    "dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c",
    addTrackers: new[] { "wss://my-private-tracker.example.com" }
);

// Check if a string is a magnet URI or info hash
bool isMagnet   = WebTorrentService.IsMagnet(input);
bool isInfoHash = WebTorrentService.IsInfoHash(input);
```

#### Speed Throttling

Global upload/download speed limits can be set on the `WebTorrent` client at any time:

```csharp
// Limit download to 500 KB/s
webTorrentService.Client!.ThrottleDownload(500 * 1024);

// Limit upload to 100 KB/s
webTorrentService.Client!.ThrottleUpload(100 * 1024);

// Disable throttling
webTorrentService.Client!.ThrottleDownload(-1);
webTorrentService.Client!.ThrottleUpload(-1);
```

#### DI-Registered Wire Extensions (`IExtensionFactory`)

Custom BitTorrent wire protocol extensions can now be registered via the DI container. Every new peer wire automatically gets your extension without any manual wiring:

```csharp
// MyExtensionFactory implements IExtensionFactory
builder.Services.AddSingleton<IExtensionFactory, MyExtensionFactory>();
builder.Services.AddWebTorrentService();
```

```csharp
public class MyExtensionFactory : IExtensionFactory
{
    public string ExtensionName => "my_ext";

    public Extension CreateExtension(Torrent torrent, Wire wire)
    {
        var ext = new SimpleExtension(torrent, wire, ExtensionName);
        ext.OnMessageReceived += (sender, data) =>
        {
            Console.WriteLine($"Received {data.ByteLength} bytes from {wire.PeerId}");
        };
        return ext;
    }
}
```

`SimpleExtension` handles BEP-10 extended-handshake negotiation, message sending/receiving, and wire lifecycle automatically.

#### Torrent Selection and Piece Control

New convenience methods on `Torrent` complement the existing file-level `Select`/`Deselect`:

```csharp
// Select all files
torrent.SelectAll();

// Deselect all files
torrent.DeselectAll();

// Prioritise critical pieces (e.g. for seeking)
torrent.Critical(startPiece, endPiece);
```

#### `File.Text()` Helper

Read an entire text file from a torrent as a `string` without intermediate `Blob` handling:

```csharp
string content = await file.Text();
```

#### Built-in Public Domain Demo Magnets

`WebTorrentService.CCMagnets` provides a ready-to-use dictionary of Creative Commons torrents
(Big Buck Bunny, Cosmos Laundromat, Sintel, Tears of Steel) for testing and demos.

---

### Breaking Changes from v1.x

| Area | v1.x | v2.x |
|---|---|---|
| **SpawnDev.BlazorJS** dependency | 2.x | **3.0.2** |
| **Target frameworks** | net6.0 / net7.0 / net8.0 | **net8.0 · net9.0 · net10.0** |
| **Static web asset path** | `/_content/SpawnDev.BlazorJS.WebTorrents/` | **`/`** (root) — required for Service Worker scope |
| **`Torrent.WhenReady`** | Not available | Added as `async Task` with optional `CancellationToken` / timeout |
| **Recent torrents** | Not available | Enabled by default (`EnableRecent = true`) |

> **Service worker path change**: Because `StaticWebAssetBasePath` is now `/`, the bundled scripts (`webtorrent.min.js`, `sw.min.js`) are served from the app root rather than `/_content/SpawnDev.BlazorJS.WebTorrents/`. Update any hard-coded paths in your `service-worker.js` or `index.html` accordingly.

---

### Upgrade Guide (v1.x → v2.0.2)

**1. Update the NuGet package**

```bash
dotnet add package SpawnDev.BlazorJS.WebTorrents --version 2.0.2
```

**2. Update SpawnDev.BlazorJS**

```bash
dotnet add package SpawnDev.BlazorJS --version 3.0.2
```

**3. Update your `index.html` service worker shim** (if using streaming)

The scripts are now at the root, not under `_content/...`:

```js
// Before (v1.x)
self.importScripts('_content/SpawnDev.BlazorJS.WebTorrents/sw.min.js');

// After (v2.x)
self.importScripts('sw.min.js');
```

**4. `Program.cs` — no changes required**

The `AddWebTorrentService()` / `AddBlazorJSRuntime()` / `BlazorJSRunAsync()` registration pattern is unchanged.

**5. Review the `EnableRecent` default**

Recent-torrent persistence is **enabled by default** (`EnableRecent = true`). If you do not want torrents automatically reloaded on startup, opt out explicitly:

```csharp
builder.Services.AddWebTorrentService(configureCallback: svc =>
{
    svc.EnableRecent = false;
});
```

---

### Full API Quick Reference

#### `WebTorrentService` (injectable singleton)

```csharp
await WebTorrentService.Ready;               // wait for client init
WebTorrent client = WebTorrentService.Client!;

// Events
WebTorrentService.OnTorrentAdd    += (t) => { };
WebTorrentService.OnTorrentRemove += (t) => { };
WebTorrentService.OnTorrentWireAdd    += (t, w) => { };
WebTorrentService.OnTorrentWireRemove += (t, w) => { };

// Utilities
bool started = await WebTorrentService.EnableServer();
bool exists  = await WebTorrentService.GetTorrentExists(infoHash);
Torrent? t   = await WebTorrentService.GetTorrent(magnetUri);
int removed  = WebTorrentService.RemoveAllTorrents(confirmed: true);
int removed2 = WebTorrentService.RemoveCompleted(confirmed: true);
string poster = await WebTorrentService.GetTorrentPoster(torrent);
```

#### `WebTorrent` (client)

```csharp
Torrent t  = client.Add(magnetUriOrInfoHashOrBytes);
Torrent t2 = client.Add(magnetUri, new AddTorrentOptions { Paused = true });
Torrent t3 = await client.AddAsync(magnetUri);

Torrent s  = client.Seed(blob);
Torrent s2 = await client.SeedAsync(blob, new SeedTorrentOptions { Name = "My File" });

Torrent? found = await client.Get(infoHash);
await client.Remove(infoHash);
await client.Remove(infoHash, new DestroyTorrentOptions { DestroyStore = true });

client.ThrottleDownload(bytesPerSec);
client.ThrottleUpload(bytesPerSec);

Array<Torrent> all = client.Torrents;
double dlSpeed  = client.DownloadSpeed;
double ulSpeed  = client.UploadSpeed;
double progress = client.Progress;
```

#### `Torrent`

```csharp
// Identity
string name      = torrent.Name;
string infoHash  = torrent.InfoHash;
string magnetURI = torrent.MagnetURI;
string id        = torrent.InstanceId;  // stable in-process key

// Status
bool   ready    = torrent.Ready;
bool   done     = torrent.Done;
bool   paused   = torrent.Paused;
double progress = torrent.Progress;   // 0–1
long   length   = torrent.Length;
long   downloaded = torrent.Downloaded;
long   uploaded   = torrent.Uploaded;
double dlSpeed    = torrent.DownloadSpeed;
double ulSpeed    = torrent.UploadSpeed;
int    peers      = torrent.NumPeers;
double? timeLeft  = torrent.TimeRemaining; // ms

// Files
Array<File> files = torrent.Files;

// Control
torrent.Pause();
torrent.Resume();
torrent.SelectAll();
torrent.DeselectAll();
torrent.Destroy();
await torrent.DestroyAsync();
await torrent.DestroyAsync(new DestroyTorrentOptions { DestroyStore = true });

// Async ready wait
await torrent.WhenReady();
await torrent.WhenReady(cts.Token);
await torrent.WhenReady(timeoutMS: 5_000);

// Events
torrent.OnReady    += () => { };
torrent.OnDone     += () => { };
torrent.OnDownload += (bytes) => { };
torrent.OnUpload   += (bytes) => { };
torrent.OnWire     += (wire)  => { };
torrent.OnError    += (err)   => { };
torrent.OnClose    += () => { };
```

#### `File`

```csharp
string name     = file.Name;
string path     = file.Path;
long   length   = file.Length;
double progress = file.Progress;   // 0–1
long   downloaded = file.Downloaded;
bool   done     = file.IsDone();   // extension method

// Select / deselect for download
file.Select();
file.Deselect();

// Read
string      text   = await file.Text();
ArrayBuffer buf    = await file.ArrayBuffer();
Blob        blob   = await file.Blob();
ReadableStream rs  = file.CreateReadStream();

// Service Worker streaming (requires CreateServer)
string url = file.StreamURL;
file.StreamTo(videoElementRef);   // ElementReference or JSObject
```

---

### Known Issues / Notes

- **Blazor WebAssembly only** – The library does not support Blazor Server or any non-browser runtime. `WebTorrentService` silently no-ops outside the `Window` scope (e.g. in Web Workers).
- **HTTPS required in production** – WebRTC peer connections require a secure context. `localhost` works without HTTPS during development.
- **Dispose JSObject wrappers** – `Torrent`, `File`, `Wire`, and other `JSObject` subclasses hold references to JavaScript objects. Always `Dispose()` them when they go out of scope to avoid memory leaks, or use `.Using()` / `.UsingEach()` helpers from SpawnDev.BlazorJS.
- **Service Worker scope** – Because `StaticWebAssetBasePath` is `/`, the service worker registration scope is the entire origin. Ensure no other service workers conflict.
- **IndexedDB storage** – Downloaded pieces are stored in the browser's OPFS by default. Clearing site data in the browser will remove downloaded content.

---

### Resources

- [Live Demo](https://lostbeard.github.io/SpawnDev.BlazorJS.WebTorrents/)
- [NuGet Package](https://www.nuget.org/packages/SpawnDev.BlazorJS.WebTorrents)
- [GitHub Repository](https://github.com/LostBeard/SpawnDev.BlazorJS.WebTorrents)
- [SpawnDev.BlazorJS](https://github.com/LostBeard/SpawnDev.BlazorJS)
- [WebTorrent JS API Docs](https://github.com/webtorrent/webtorrent/blob/master/docs/api.md)
- [WebTorrent JS Changelog](https://github.com/webtorrent/webtorrent/blob/master/CHANGELOG.md)
