# SpawnDev.BlazorJS.WebTorrents v2.0.2

[![NuGet](https://badge.fury.io/nu/SpawnDev.BlazorJS.WebTorrents.svg)](https://www.nuget.org/packages/SpawnDev.BlazorJS.WebTorrents)
&nbsp;**Targets:** .NET 8 · .NET 9 · .NET 10 &nbsp;|&nbsp; **Bundled WebTorrent JS:** 2.8.3

Bring the full [WebTorrent](https://github.com/webtorrent/webtorrent) library into your Blazor WebAssembly app with a strongly-typed C# API. Download, seed, and — most importantly — **stream torrent data directly in the browser as pieces arrive**, without waiting for a complete download.

[Live Demo](https://lostbeard.github.io/SpawnDev.BlazorJS.WebTorrents/) · [GitHub](https://github.com/LostBeard/SpawnDev.BlazorJS.WebTorrents) · [NuGet](https://www.nuget.org/packages/SpawnDev.BlazorJS.WebTorrents)

---

## Quick Start

```bash
dotnet add package SpawnDev.BlazorJS.WebTorrents
```

```csharp
// Program.cs
builder.Services.AddBlazorJSRuntime();
builder.Services.AddWebTorrentService();
await builder.Build().BlazorJSRunAsync();
```

---

## Streaming Torrent Data While It Downloads

The headline capability: torrent files can be read, played, or displayed **as soon as the first pieces arrive** — no full download required.

### Stream video / audio to a browser element

Enable the bundled Service Worker HTTP server once (e.g. in `Program.cs` or a startup service):

```csharp
// wwwroot/service-worker.js must contain:
//   self.importScripts('sw.min.js');
await webTorrentService.EnableServer();
```

Then point any media element straight at a torrent file:

```razor
@inject WebTorrentService WebTorrentService

<video @ref="videoRef" controls autoplay></video>

@code {
    ElementReference videoRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await WebTorrentService.Ready;
        // replace with a real magnet URI, info hash, or .torrent URL
        var torrent = WebTorrentService.Client!.Add("magnet:?xt=urn:btih:...");
        await torrent.WhenReady();
        var video = torrent.Files.ToArray().First(f => f.Name.EndsWith(".mp4"));
        video.StreamTo(videoRef);   // starts playing immediately as pieces arrive
    }
}
```

`File.StreamTo()` uses HTTP range requests served by the Service Worker so the browser can seek, buffer, and decode the file exactly as if it were a normal hosted resource.

### Get a streaming URL directly

```csharp
// Use with <video src="@streamUrl" /> or fetch()
string streamUrl = file.StreamURL;
```

### Read data as a stream or buffer

Access file data progressively without waiting for a complete download:

```csharp
// Readable stream — pieces are fetched on demand
ReadableStream stream = file.CreateReadStream();

// Full file as ArrayBuffer or Blob (awaits completion of needed pieces)
ArrayBuffer buffer = await file.ArrayBuffer();
Blob        blob   = await file.Blob();
string      text   = await file.Text();   // convenience wrapper for text files
```

---

## Key Features

### Download and seed from anywhere

```csharp
await WebTorrentService.Ready;

// Add by magnet URI, info hash, .torrent URL, or raw bytes — substitute your own
// or use one of the built-in CC magnets: WebTorrentService.CCMagnets["Big Buck Bunny"]
Torrent torrent = WebTorrentService.Client!.Add("magnet:?xt=urn:btih:...");

// Seed files from the browser
Torrent seeded = WebTorrentService.Client!.Seed(blob, new SeedTorrentOptions { Name = "MyFile" });
Console.WriteLine(seeded.MagnetURI);
```

### Download progress and events

```csharp
torrent.OnDownload += (bytes) =>
{
    Console.WriteLine($"{torrent.Progress * 100:F1}%  {torrent.DownloadSpeed / 1024:F0} KB/s  {torrent.NumPeers} peers");
};
torrent.OnDone += () => Console.WriteLine("Complete!");
```

### Select only the files you need

```csharp
await torrent.WhenReady();   // metadata is ready; files are enumerable
foreach (var file in torrent.Files.ToArray())
{
    if (file.Name.EndsWith(".mp4"))
        file.Select();    // only download this file
    else
        file.Deselect();
}
```

### Persistent recent torrents

Torrent metadata is saved to OPFS and reloaded automatically on the next app start:

```csharp
builder.Services.AddWebTorrentService(configureCallback: svc =>
{
    svc.EnableRecent = true;          // default
    svc.LoadRecentPaused = true;      // reload paused, don't auto-resume
});
```

### Wire protocol extensions

Register custom BitTorrent protocol extensions via DI — they are applied to every new peer connection automatically:

```csharp
builder.Services.AddSingleton<IExtensionFactory, MyExtensionFactory>();
builder.Services.AddWebTorrentService();
```

---

## Notes

- **Blazor WebAssembly only** — does not run in Blazor Server or Web Workers.
- **HTTPS required in production** — WebRTC requires a secure context (`localhost` is exempt).
- **Dispose `JSObject` wrappers** — `Torrent`, `File`, `Wire`, etc. wrap live JS references; call `Dispose()` or use the `.Using()` / `.UsingEach()` helpers provided by SpawnDev.BlazorJS.
- Downloaded pieces are stored in the browser's **Origin Private File System (OPFS)**. Clearing site data will remove them.

---

## Resources

- [Live Demo](https://lostbeard.github.io/SpawnDev.BlazorJS.WebTorrents/)
- [NuGet Package](https://www.nuget.org/packages/SpawnDev.BlazorJS.WebTorrents)
- [GitHub Repository](https://github.com/LostBeard/SpawnDev.BlazorJS.WebTorrents)
- [SpawnDev.BlazorJS](https://github.com/LostBeard/SpawnDev.BlazorJS)
- [WebTorrent JS API Docs](https://github.com/webtorrent/webtorrent/blob/master/docs/api.md)
