using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Tracker client class<br />
    /// https://github.com/webtorrent/bittorrent-tracker
    /// </summary>
    public class Tracker : EventEmitter
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Tracker(IJSInProcessObjectReference _ref) : base(_ref) { }
        public JSEventCallback<JSObject> OnWarning { get => new JSEventCallback<JSObject>("warning", On, RemoveListener); set { } }
        public JSEventCallback<JSObject> OnError { get => new JSEventCallback<JSObject>("error", On, RemoveListener); set { } }
        public JSEventCallback<DiscoveredPeer> OnPeer { get => new JSEventCallback<DiscoveredPeer>("peer", On, RemoveListener); set { } }
        public JSEventCallback<TrackerUpdateMessage> OnUpdate { get => new JSEventCallback<TrackerUpdateMessage>("update", On, RemoveListener); set { } }
        public string UserAgent => JSRef.Get<string>("_userAgent");
        public Array<TrackerConnection> Trackers => JSRef.Get<Array<TrackerConnection>>("_trackers");
        public void Start() => JSRef.CallVoid("start");
        public void Complete() => JSRef.CallVoid("complete");
        public void Update() => JSRef.CallVoid("update");
        public void Stop() => JSRef.CallVoid("stop");
        public void Destroy() => JSRef.CallVoid("destroy");
        public void Scrape() => JSRef.CallVoid("scrape");
    }
}
