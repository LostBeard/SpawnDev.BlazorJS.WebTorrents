using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.WebTorrents;
using SpawnDev.BlazorJS.WebTorrents.Demo;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddBlazorJSRuntime();
builder.Services.AddWebTorrentService(webTorrentService =>
{
    webTorrentService.EnableRecent = true;
    webTorrentService.LoadRecentDeselected = true;
});

builder.Services.AddRadzenComponents();

await builder.Build().BlazorJSRunAsync();
