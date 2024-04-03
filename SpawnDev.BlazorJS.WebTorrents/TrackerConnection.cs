using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Represents a Tracker client connection to a tracker
    /// </summary>
    public class TrackerConnection : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public TrackerConnection(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Tracker url
        /// </summary>
        public string AnnounceUrl => JSRef.Get<string>("announceUrl");
        /// <summary>
        /// True if the tracker connection has been destroyed
        /// </summary>
        public bool Destroyed => JSRef.Get<bool>("destroyed");
        /// <summary>
        /// Returns true if the expectign a response
        /// </summary>
        public bool ExpectingResponse => JSRef.Get<bool>("expectingResponse");
        /// <summary>
        /// Returns true if reconnecting
        /// </summary>
        public bool Reconnecting => JSRef.Get<bool>("reconnecting");
        /// <summary>
        /// The number of reconnect retries
        /// </summary>
        public int Retries => JSRef.Get<int>("retries");
    }
}
