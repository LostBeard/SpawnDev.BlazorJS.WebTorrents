using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class Peer : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Peer(IJSInProcessObjectReference _ref) : base(_ref) { }
        public string Id => JSRef.Get<string>("id");
        public string Type => JSRef.Get<string>("type");
        public bool Destroyed => JSRef.Get<bool>("destroyed");
        public bool Connected => JSRef.Get<bool>("connected");
        public bool SendHandshake => JSRef.Get<bool>("sendHandshake");
        public int HandshakeTimeout => JSRef.Get<int>("handshakeTimeout");
        public int Retries => JSRef.Get<int>("retries");
        public Wire? Wire => JSRef.Get<Wire>("wire");
        public Torrent? Swarm => JSRef.Get<Torrent?>("swarm");
    }
}
