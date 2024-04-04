using Microsoft.JSInterop;
using static SpawnDev.BlazorJS.WebTorrents.Demo.Services.FetchStatsService;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Services
{
    public class FetchStats : JSObject
    {
        public FetchStats(IJSInProcessObjectReference _ref) : base(_ref) { }
        public Dictionary<string, HostPing> HostPings => JSRef.Get<Dictionary<string, HostPing>>("hostPings");
        public bool IsHostBlocked(string hostname) => JSRef.Call<bool>("isHostBlocked", hostname);
        public bool BlockHost(string hostname) => JSRef.Call<bool>("blockHost", hostname);
        public bool UnblockHost(string hostname) => JSRef.Call<bool>("unblockHost", hostname);
    }
}
