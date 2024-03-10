using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.Toolbox;
using SpawnDev.BlazorJS.WebTorrents;
using SpawnDev.BlazorJS.WebTorrents.Demo;
using SpawnDev.BlazorJS.WebTorrents.WireExtensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazorJSRuntime();

builder.Services.AddSingleton<BeforeUnloadService>();

builder.Services.AddSingleton<WebTorrentService>();


//builder.Services.AddSingleton(o => new WireExtensionFactory<TestWireExtension>("mysuperext"));

builder.Services.AddSingleton<RendezvousWireExtensionFactory>(); 

await builder.Build().BlazorJSRunAsync();
