using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Piece class
    /// https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#piece-api
    /// </summary>
    public class Piece : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Piece(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Piece length (in bytes). Example: 12345
        /// </summary>
        public int Length => JSRef.Get<int>("length");
        /// <summary>
        /// Piece missing length (in bytes). Example: 100
        /// </summary>
        public int Missing => JSRef.Get<int>("length");
    }
}
