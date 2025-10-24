using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.WebTorrents;
using SpawnDev.BlazorJS.WebTorrents.Demo;
using SpawnDev.BlazorJS.WebTorrents.Demo.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddBlazorJSRuntime();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddWebTorrentService(new WebTorrentOptions
{
    //DownloadLimit = 50000,
    Tracker = new TrackerClientOptions
    {
        // optionally set the default trackers to use for seeding
        //Announce = new string[] { "wss://pi.spawndev.com:44365", "wss://psi.spawndev.com:44365" },
    }
}, webTorrentService =>
{
    webTorrentService.Verbose = true;
});
builder.Services.AddSingleton<FetchStatsService>();
builder.Services.AddSingleton<MimeTypeService>();
builder.Services.AddSingleton<FetchStatsService>();
// DHTFactory is an IExtensionFactory.
// WebTorrentService will call IExtensionFactory.Create on all services implementing IExtensionFactory when a new wire connects
// to implementing wire extensions
// DHTFactory is currently for demonstration purposes only and does not actually add dht support
//builder.Services.AddSingleton<DHTFactory>();
builder.Services.AddSingleton<AppService>();
builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddRadzenComponents();
var host =   await builder.Build().StartBackgroundServices();

#if DEBUG


#endif
await host.BlazorJSRunAsync();
