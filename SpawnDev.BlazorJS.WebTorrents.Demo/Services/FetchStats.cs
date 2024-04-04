using Microsoft.JSInterop;
using static SpawnDev.BlazorJS.WebTorrents.Demo.Services.FetchStatsService;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Services
{
    public class FetchStats : JSObject
    {
        public FetchStats(IJSInProcessObjectReference _ref) : base(_ref) { }
        public Dictionary<string, OriginPing> OriginPings => JSRef.Get<Dictionary<string, OriginPing>>("originPings");
        public bool IsBlockedHost(string hostname) => JSRef.Call<bool>("isBlockedHost", hostname);
        public bool BlockHost(string hostname) => JSRef.Call<bool>("blockHost", hostname);
        public bool UnblockHost(string hostname) => JSRef.Call<bool>("unblockHost", hostname);
    }
}
