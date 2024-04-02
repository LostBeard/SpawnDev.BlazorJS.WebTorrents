using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class TrackerConnection : JSObject
    {
        public TrackerConnection(IJSInProcessObjectReference _ref) : base(_ref) { }
        public string AnnounceUrl => JSRef.Get<string>("announceUrl");
        public bool Destroyed => JSRef.Get<bool>("destroyed");
        public bool ExpectingResponse => JSRef.Get<bool>("expectingResponse");
        public bool Reconnecting => JSRef.Get<bool>("reconnecting");
        public int Retries => JSRef.Get<int>("retries");
    }
}
