using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// TorrentStore object
    /// </summary>
    public class TorrentStore : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public TorrentStore(IJSInProcessObjectReference _ref) : base(_ref) { }
    }
}
