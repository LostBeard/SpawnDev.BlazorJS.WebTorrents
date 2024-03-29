using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class PeerPieces : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public PeerPieces(IJSInProcessObjectReference _ref) : base(_ref) { }
        public Uint8Array Buffer => JSRef.Get<Uint8Array>("buffer");
        public int Grow => JSRef.Get<int>("grow");
    }
}
