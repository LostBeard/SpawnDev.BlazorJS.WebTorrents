# SpawnDev.BlazorJS.WebTorrents

[![NuGet version](https://badge.fury.io/nu/SpawnDev.BlazorJS.WebTorrents.svg?label=SpawnDev.BlazorJS.WebTorrents)](https://www.nuget.org/packages/SpawnDev.BlazorJS.WebTorrents)

**SpawnDev.BlazorJS.WebTorrents** brings the amazing [WebTorrent](https://github.com/webtorrent/webtorrent) library to Blazor WebAssembly.

[WebTorrent](https://github.com/webtorrent/webtorrent) is a streaming torrent client for node.js and the browser. YEP, THAT'S RIGHT. THE BROWSER. It's written completely in JavaScript – the language of the web – so the same code works in both runtimes.

## Features

- 🚀 **Streaming torrents** in your Blazor WebAssembly applications
- 📦 **Full WebTorrent API** access through strongly-typed C# wrappers
- 🎬 **Stream video and audio** directly from torrents
- 📁 **Download and seed files** entirely in the browser
- 🔄 **Real-time progress tracking** with events
- 💾 **IndexedDB storage** support for persistent downloads
- 🎯 **Selective file downloading** - choose which files to download
- 🌐 **WebRTC peer-to-peer** connections
- 📊 **Statistics and metrics** - track download/upload speeds, peer counts, etc.
- 🎨 **Intellisense support** with comprehensive XML documentation

## Demo
[Live Demo](https://lostbeard.github.io/SpawnDev.BlazorJS.WebTorrents/)

## Prerequisites

- .NET 8.0 or later
- Blazor WebAssembly project
- Modern web browser with WebRTC support
- [SpawnDev.BlazorJS](https://www.nuget.org/packages/SpawnDev.BlazorJS) (automatically installed as a dependency)

## Installation

### 1. Install the NuGet package

```bash
dotnet add package SpawnDev.BlazorJS.WebTorrents
```

Or via Package Manager Console:

```powershell
Install-Package SpawnDev.BlazorJS.WebTorrents
```

### 2. Configure your `Program.cs`

Update your `Program.cs` to register the required services:

```csharp
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.WebTorrents;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register BlazorJSRuntime (required)
builder.Services.AddBlazorJSRuntime();

// Register WebTorrent service
builder.Services.AddWebTorrentService();

// Initialize BlazorJSRuntime instead of standard RunAsync()
await builder.Build().BlazorJSRunAsync();
```

### 3. Advanced Configuration (Optional)

You can customize WebTorrent behavior with options:

```csharp
builder.Services.AddWebTorrentService(new WebTorrentOptions
{
    DownloadLimit = 50000,  // Download speed limit in bytes/sec
    Tracker = new TrackerClientOptions
    {
        // Set default trackers for seeding
        Announce = new string[] 
        { 
            "wss://tracker.webtorrent.dev",
            "wss://tracker.openwebtorrent.com"
        },
    }
}, webTorrentService =>
{
    // Enable verbose logging
    webTorrentService.Verbose = true;
});
```

## Quick Start

### Basic Usage in a Razor Component

```razor
@page "/torrents"
@inject WebTorrentService WebTorrentService
@implements IDisposable

<h3>Torrent Downloader</h3>

@if (torrent != null)
{
    <div>
        <p>Name: @torrent.Name</p>
        <p>Progress: @((torrent.Progress * 100).ToString("F2"))%</p>
        <p>Download Speed: @FormatBytes(torrent.DownloadSpeed)/s</p>
        <p>Upload Speed: @FormatBytes(torrent.UploadSpeed)/s</p>
        <p>Peers: @torrent.NumPeers</p>
    </div>
}

@code {
    private Torrent? torrent;

    protected override async Task OnInitializedAsync()
    {
        // Wait for WebTorrent to be ready
        await WebTorrentService.Ready;

        // Add a torrent
        var magnetUri = "magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c"; // Big Buck Bunny
        torrent = WebTorrentService.Client!.Add(magnetUri);

        // Subscribe to events
        torrent.On("download", () => StateHasChanged());
        torrent.On("upload", () => StateHasChanged());
    }

    private string FormatBytes(double bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        while (bytes >= 1024 && order < sizes.Length - 1)
        {
            order++;
            bytes /= 1024;
        }
        return $"{bytes:F2} {sizes[order]}";
    }

    public void Dispose()
    {
        torrent?.Dispose();
    }
}
```

## Common Usage Examples

### Download a Torrent

```csharp
await WebTorrentService.Ready;

var torrent = WebTorrentService.Client!.Add("magnet:?xt=urn:btih:...");

torrent.On("ready", () =>
{
    Console.WriteLine($"Torrent ready: {torrent.Name}");
    Console.WriteLine($"Files: {torrent.Files.Length}");
});

torrent.On("done", () =>
{
    Console.WriteLine("Download complete!");
});
```

### Seed a File

```csharp
await WebTorrentService.Ready;

// Create a torrent from files
using var fileInput = new HTMLInputElement();
fileInput.Type = "file";
fileInput.Multiple = true;
fileInput.Click();

fileInput.OnChange += async () =>
{
    var files = fileInput.Files!;
    var torrent = WebTorrentService.Client!.Seed(files);

    torrent.On("ready", () =>
    {
        Console.WriteLine($"Seeding: {torrent.MagnetURI}");
    });
};
```

### Stream Video from a Torrent

```csharp
await WebTorrentService.Ready;

var torrent = WebTorrentService.Client!.Add(magnetUri);

torrent.On("ready", () =>
{
    // Find the video file
    var videoFile = torrent.Files.FirstOrDefault(f => 
        f.Name.EndsWith(".mp4") || f.Name.EndsWith(".webm"));

    if (videoFile != null)
    {
        // Create a blob URL for the video
        var blobUrl = videoFile.StreamURL;

        // Use with HTML video element
        // <video src="@blobUrl" controls></video>
    }
});
```

### Download Specific Files Only

```csharp
var torrent = WebTorrentService.Client!.Add(magnetUri, new TorrentOptions
{
    // Deselect all files initially
    DeselectedFiles = true
});

torrent.On("ready", () =>
{
    // Select only specific files to download
    foreach (var file in torrent.Files)
    {
        if (file.Name.EndsWith(".mp4"))
        {
            file.Select();
        }
    }
});
```

### Track Download Progress

```csharp
var torrent = WebTorrentService.Client!.Add(magnetUri);

torrent.On("download", () =>
{
    Console.WriteLine($"Progress: {torrent.Progress * 100:F2}%");
    Console.WriteLine($"Downloaded: {torrent.Downloaded} bytes");
    Console.WriteLine($"Speed: {torrent.DownloadSpeed} bytes/sec");
    Console.WriteLine($"Peers: {torrent.NumPeers}");
    Console.WriteLine($"Time remaining: {torrent.TimeRemaining} ms");
});
```

### Remove/Destroy a Torrent

```csharp
// Remove torrent (keeps files)
WebTorrentService.Client!.Remove(torrent);

// Destroy torrent (deletes files)
await torrent.DestroyAsync();
```

## API Overview

### WebTorrentService

The main service for managing torrents.

**Key Properties:**
- `Client` - The WebTorrent client instance
- `Ready` - Task that completes when the service is initialized
- `Verbose` - Enable verbose logging

**Key Events:**
- `OnTorrentAdd` - Fired when a torrent is added
- `OnTorrentRemove` - Fired when a torrent is removed
- `OnTorrentWireAdd` - Fired when a peer connection is established
- `OnTorrentWireRemove` - Fired when a peer connection is closed

### Torrent

Represents a torrent.

**Key Properties:**
- `Name` - Torrent name
- `InfoHash` - Info hash
- `MagnetURI` - Magnet URI
- `Files` - Array of files in the torrent
- `Progress` - Download progress (0-1)
- `DownloadSpeed` - Current download speed in bytes/sec
- `UploadSpeed` - Current upload speed in bytes/sec
- `Downloaded` - Total bytes downloaded
- `Uploaded` - Total bytes uploaded
- `NumPeers` - Number of connected peers
- `Ready` - True when metadata is loaded

**Key Methods:**
- `Select()` - Resume downloading
- `Deselect()` - Pause downloading
- `DestroyAsync()` - Destroy torrent and delete files
- `Pause()` - Pause the torrent
- `Resume()` - Resume the torrent

**Key Events:**
- `ready` - Fired when torrent metadata is ready
- `done` - Fired when all files are downloaded
- `download` - Fired on download progress
- `upload` - Fired on upload progress
- `wire` - Fired when a new peer connects

### File

Represents a file within a torrent.

**Key Properties:**
- `Name` - File name
- `Path` - File path
- `Length` - File size in bytes
- `Downloaded` - Bytes downloaded
- `Progress` - Download progress (0-1)
- `StreamURL` - Blob URL for streaming

**Key Methods:**
- `Select()` - Mark file for download
- `Deselect()` - Exclude file from download
- `GetBlobURL()` - Get a blob URL for the file
- `GetBlob()` - Get file as a Blob

## Documentation

SpawnDev.BlazorJS.WebTorrents is a collection of [JSObject](https://github.com/LostBeard/SpawnDev.BlazorJS?tab=readme-ov-file#jsobject-base-class) wrappers that provide strongly-typed access to the JavaScript [WebTorrent](https://github.com/webtorrent/webtorrent) library. The C# API closely mirrors the JavaScript API with full IntelliSense support.

**Additional Resources:**
- [WebTorrent JavaScript API Documentation](https://github.com/webtorrent/webtorrent/blob/master/docs/api.md)
- [SpawnDev.BlazorJS Documentation](https://github.com/LostBeard/SpawnDev.BlazorJS)

## Browser Support

WebTorrent (and thus this library) requires WebRTC support. All modern browsers support WebRTC:

- ✅ Chrome/Edge (Chromium-based)
- ✅ Firefox
- ✅ Safari
- ✅ Opera

## Important Notes

- **Blazor WebAssembly Only**: This library only works in Blazor WebAssembly projects. It will not work in Blazor Server.
- **Resource Management**: Always dispose of `Torrent` and `File` objects when done to free up resources.
- **HTTPS Required**: WebRTC requires HTTPS in production environments (localhost works with HTTP during development).
- **Storage**: Downloaded files are stored in the browser's IndexedDB by default.

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests at the [GitHub repository](https://github.com/LostBeard/SpawnDev.BlazorJS.WebTorrents).

## Related Projects

- [SpawnDev.BlazorJS](https://github.com/LostBeard/SpawnDev.BlazorJS) - The foundation library for Blazor WebAssembly JavaScript interop
- [WebTorrent](https://github.com/webtorrent/webtorrent) - The original JavaScript WebTorrent library

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## Acknowledgments

- Built on top of the excellent [WebTorrent](https://github.com/webtorrent/webtorrent) library
- Powered by [SpawnDev.BlazorJS](https://github.com/LostBeard/SpawnDev.BlazorJS)

