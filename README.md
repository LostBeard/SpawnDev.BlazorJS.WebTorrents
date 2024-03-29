# SpawnDev.BlazorJS.WebTorrents

[![NuGet version](https://badge.fury.io/nu/SpawnDev.BlazorJS.WebTorrents.svg)](https://www.nuget.org/packages/SpawnDev.BlazorJS.WebTorrents)

[WebTorrent](https://github.com/webtorrent/webtorrent) for Blazor WebAssembly

[WebTorrent](https://github.com/webtorrent/webtorrent) is a streaming torrent client for node.js and the browser. YEP, THAT'S RIGHT. THE BROWSER. It's written completely in JavaScript – the language of the web – so the same code works in both runtimes.

### Documentation
SpawnDev.BlazorJS.WebTorrents is a collection of [JSObject](https://github.com/LostBeard/SpawnDev.BlazorJS?tab=readme-ov-file#jsobject-base-class) wrappers that allow access to the Javascript [WebTorrent](https://github.com/webtorrent/webtorrent) library. The interfaces are nearly identical. Intellisense documentation is included.   

**[WebTorrent API Documentation](https://github.com/webtorrent/webtorrent/blob/master/docs/api.md)** 

### Demo
[Live Demo](https://lostbeard.github.io/SpawnDev.BlazorJS.WebTorrents/)

### Getting started

Example Program.cs 
```cs
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.WebTorrents;
using SpawnDev.BlazorJS.WebTorrents.Demo;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
// Add BlazorJSRunAsync service
builder.Services.AddBlazorJSRuntime();
// Add WebTorrentService service
builder.Services.AddWebTorrentService();
// initialize BlazorJSRuntime to start app
await builder.Build().BlazorJSRunAsync();
```

Inject
```cs
[Inject] WebTorrentService WebTorrentService { get; set; }
```

WebTorrentService.Client is an instance of [WebTorrent](https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#webtorrent-api)

Very basic example that adds a torrent magnet, waits for the torrent metadata to be retrieved and shows information about the torrents files. Then, the torrent and all related data is destroyed.
```cs
var addOptions = new AddTorrentOptions { Deselect = true };
using var torrent = WebTorrentService.Client!.Add("magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c&dn=Big+Buck+Bunny&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fbig-buck-bunny.torrent", addOptions);
await torrent.WhenReady();
Console.WriteLine($"InfoHash: {torrent.InfoHash}");
Console.WriteLine($"Files: {torrent.Files.Length}");
foreach (File file in torrent.Files)
{
    Console.WriteLine($"File: {file.Name} {file.Size}");
}
await torrent.DestroyAsync(new DestroyTorrentOptions { DestroyStore = true });
```
