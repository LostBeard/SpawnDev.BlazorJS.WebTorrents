using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Represents a Tracker client connection to a tracker
    /// </summary>
    public class WebSocketTracker : Tracker
    {
        /// <summary>
        /// Returns true if the tracker looks like this type
        /// </summary>
        /// <param name="tracker"></param>
        /// <returns></returns>
        public static bool IsThisTackerType(Tracker tracker) => !tracker.JSRef!.PropertyIsUndefined("expectingResponse") && !tracker.JSRef!.PropertyIsUndefined("peers");
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public WebSocketTracker(IJSInProcessObjectReference _ref) : base(_ref) { }
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
        /// <summary>
        /// Array of Peers
        /// </summary>
        public Array<Peer> Peers => JSRef.Get<Array<Peer>>("peers");
    }
}
