using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Tracker client class<br />
    /// https://github.com/webtorrent/bittorrent-tracker
    /// </summary>
    public class Client : EventEmitter
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Client(IJSInProcessObjectReference _ref) : base(_ref) { }
        public ActionEvent<JSObject> OnWarning { get => new ActionEvent<JSObject>("warning", On, RemoveListener); set { } }
        public ActionEvent<JSObject> OnError { get => new ActionEvent<JSObject>("error", On, RemoveListener); set { } }
        //public ActionEvent<DiscoveredPeer> OnPeer { get => new ActionEvent<DiscoveredPeer>("peer", On, RemoveListener); set { } }
        // HTTPTracker emits a peer event on Client with a string
        // this.client.emit('peer', `${peer.ip}:${peer.port}`)
        // WebSocketTracker emits peer with a new Peer object
        public ActionEvent<JSObject> OnPeer { get => new ActionEvent<JSObject>("peer", On, RemoveListener); set { } }
        public ActionEvent<TrackerUpdateMessage> OnUpdate { get => new ActionEvent<TrackerUpdateMessage>("update", On, RemoveListener); set { } }
        public string UserAgent => JSRef.Get<string>("_userAgent");
        public string PeerId => JSRef.Get<string>("peerId");
        public string InfoHash => JSRef.Get<string>("infoHash");
        /// <summary>
        /// True if this Client has been destroyed
        /// </summary>
        public bool Destroyed => JSRef.Get<bool>("destroyed");
        public Array<Tracker> Trackers => JSRef.Get<Array<Tracker>>("_trackers");
        public void Start() => JSRef.CallVoid("start");
        public void Complete() => JSRef.CallVoid("complete");
        public void Update() => JSRef.CallVoid("update");
        public void Stop() => JSRef.CallVoid("stop");
        public void Destroy() => JSRef.CallVoid("destroy");
        public void Scrape() => JSRef.CallVoid("scrape");
    }
}
