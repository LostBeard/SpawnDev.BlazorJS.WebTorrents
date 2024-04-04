using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class DiscoveredPeer : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public DiscoveredPeer(IJSInProcessObjectReference _ref) : base(_ref) { }
        public string Id => JSRef.Get<string>("id");
        public string ChannelName => JSRef.Get<string>("channelName");
        public bool Initiator => JSRef.Get<bool>("initiator");
        public bool Trickle => JSRef.Get<bool>("trickle");
        public string? RemoteAddress => JSRef.Get<string?>("remoteAddress");
        public string? RemoteFamily => JSRef.Get<string?>("remoteFamily");
        public ushort? RemotePort => JSRef.Get<ushort?>("remotePort");
    }
}
