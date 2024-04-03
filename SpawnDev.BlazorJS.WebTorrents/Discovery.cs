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
        public bool Destroyed => JSRef.Get<bool>("destroyed");
        public Tracker Tracker => JSRef.Get<Tracker>("tracker");
        public string PeerId => JSRef.Get<string>("peerId");
        public string InfoHash => JSRef.Get<string>("infoHash");
    }
}
