using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.WebTorrents;
using SpawnDev.BlazorJS.WebTorrents.Demo;
using System.Reflection.Metadata.Ecma335;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazorJSRuntime();
builder.Services.AddBackgroundService<WebTorrentService>();

await builder.Build().BlazorJSRunAsync();
