using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class TrackerUpdateData : JSObject
    {
        public TrackerUpdateData(IJSInProcessObjectReference _ref) : base(_ref) { }
        public string Action => JSRef.Get<string>("action");
    }
}
