using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Represents a tracker message with an unknown data type that can be determined by reading the Action property
    /// </summary>
    public class TrackerUpdateMessage : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public TrackerUpdateMessage(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Message type indicator. Can be used to determine what type of data this message contains.
        /// </summary>
        public string Action => JSRef.Get<string>("action");
        /// <summary>
        /// When Action == "announce", this message is a TrackerAnnounced message and can be accessed using AsTrackerAnnounce()
        /// </summary>
        /// <returns></returns>
        public TrackerAnnounce AsTrackerAnnounce() => JSRef.As<TrackerAnnounced>();
    }
}
