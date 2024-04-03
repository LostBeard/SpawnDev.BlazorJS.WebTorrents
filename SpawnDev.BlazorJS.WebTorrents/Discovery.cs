using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Discovery class
    /// </summary>
    public class Discovery : EventEmitter
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Discovery(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Returns true if this instance has been destroyed
        /// </summary>
        public bool Destroyed => JSRef.Get<bool>("destroyed");
        /// <summary>
        /// Torrent tracker client instance
        /// </summary>
        public Tracker Tracker => JSRef.Get<Tracker>("tracker");
        /// <summary>
        /// The client's peerId
        /// </summary>
        public string PeerId => JSRef.Get<string>("peerId");
        /// <summary>
        /// The torrent infoHash
        /// </summary>
        public string InfoHash => JSRef.Get<string>("infoHash");
        /// <summary>
        /// Emitted when there is a warning. This is purely informational and it is not necessary to listen to this event, but it may aid in debugging.
        /// </summary>
        public JSEventCallback<JSObject> OnWarning { get => new JSEventCallback<JSObject>("warning", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted on a fatal error. 
        /// </summary>
        public JSEventCallback<JSObject?> OnError { get => new JSEventCallback<JSObject?>("error", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted on tracker announce
        /// </summary>
        public JSEventCallback<TrackerUpdateMessage> OnTrackerAnnounce { get => new JSEventCallback<TrackerUpdateMessage>("trackerAnnounce", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted on dht announce
        /// </summary>
        public JSEventCallback OnDhtAnnounce { get => new JSEventCallback("dhtAnnounce", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when a peer is discovered<br />
        /// peer Peer - the peer<br />
        /// source string - the peer source
        /// </summary>
        public JSEventCallback<Peer, string> OnPeer { get => new JSEventCallback<Peer, string>("peer", On, RemoveListener); set { } }
    }
}
