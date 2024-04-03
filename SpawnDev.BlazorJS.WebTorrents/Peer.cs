using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Peer class
    /// </summary>
    public class Peer : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Peer(IJSInProcessObjectReference _ref) : base(_ref) { }
        public string ChannelName => JSRef.Get<string>("channelName");
        public string Id => JSRef.Get<string>("id");
        public bool Initiator => JSRef.Get<bool>("initiator");
        public bool Trickle => JSRef.Get<bool>("trickle");
        public string? RemoteAddress => JSRef.Get<string?>("remoteAddress");
        public string? RemoteFamily => JSRef.Get<string?>("remoteFamily");
        public ushort? RemotePort => JSRef.Get<ushort?>("remotePort");
    }
}
