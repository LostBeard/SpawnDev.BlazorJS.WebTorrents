using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class Request : JSObject
    {
        public Request(IJSInProcessObjectReference _ref) : base(_ref) { }
        public int Piece => JSRef.Get<int>("piece");
        public int Offset => JSRef.Get<int>("offset");
        public int Length => JSRef.Get<int>("length");
    }
}
