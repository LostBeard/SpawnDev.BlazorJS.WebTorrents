using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#wire-api
    // https://github.com/webtorrent/bittorrent-protocol !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public class Wire : EventEmitter
    {
        public Wire(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Remote peer id (hex string)
        /// </summary>
        public string PeerId => JSRef.Get<string>("peerId");
        /// <summary>
        /// Connection type ('webrtc', 'tcpIncoming', 'tcpOutgoing', 'utpIncoming', 'utpOutgoing', 'webSeed')
        /// </summary>
        public string Type => JSRef.Get<string>("type");
        /// <summary>
        /// Total bytes uploaded to peer.
        /// </summary>
        public long Uploaded => JSRef.Get<long>("uploaded");
        /// <summary>
        /// Total bytes downloaded from peer.
        /// </summary>
        public long Downloaded => JSRef.Get<long>("downloaded");
        /// <summary>
        /// Peer upload speed, in bytes/sec.
        /// </summary>
        /// <returns></returns>
        public double UploadSpeed => JSRef.Get<double>("uploadSpeed");
        /// <summary>
        /// Peer download speed, in bytes/sec.
        /// </summary>
        public double DownloadSpeed => JSRef.Get<double>("downloadSpeed");
        /// <summary>
        /// Peer's remote address. Only exists for tcp/utp peers.
        /// </summary>
        public string RemoteAddress => JSRef.Get<string>("remoteAddress");
        /// <summary>
        /// Peer's remote port. Only exists for tcp/utp peers.
        /// </summary>
        public int RemotePort => JSRef.Get<int>("remotePort");
        /// <summary>
        /// Close the connection with the peer. This however doesn't prevent the peer from simply re-connecting.
        /// </summary>
        public void Destroy() => JSRef.CallVoid("destroy");
        /// <summary>
        /// Tell the wire to use the given extension factory<br />
        /// An "extension factory" 
        /// </summary>
        /// <param name="extensionFactory"></param>
        public void Use(IWireExtensionFactory extensionFactory)
        {
            // the "use" method checks for extension.prototype.name
            // verify set here
            extensionFactory.SetName();
            JSRef!.CallVoid("use", extensionFactory.CreateWireExtension);
        }
        /// <summary>
        /// Send data to the named extension on the other end of the wire<br />
        /// byte arrays and Uint8Array data will be sent, without change, as a Uint8Array<br />
        /// all other data is BEncoded before being sent
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="data"></param>
        public void Extended(string extension, object data) => JSRef.CallVoid("extended", extension, data);

        public void SetKeepAlive(bool enable) => JSRef.CallVoid("setKeepAlive", enable);

        /// <summary>
        /// ExtendedHandshake properties can be set when an extension is created and those properties will be sent to peers when an extendedHandshake occurs
        /// </summary>
        public ExtendedHandshake ExtendedHandshake => JSRef.Get<ExtendedHandshake>("extendedHandshake");

        // verify below
        public bool AllowHalfOpen => JSRef.Get<bool>("allowHalfOpen");
        public bool AmChoking => JSRef.Get<bool>("amChoking");
        public bool AmInterested => JSRef.Get<bool>("amInterested");
        public bool IsSeeder => JSRef.Get<bool>("isSeeder");
        public bool PeerChoking => JSRef.Get<bool>("peerChoking");
        public bool PeerInterested => JSRef.Get<bool>("peerInterested");
        public bool Readable => JSRef.Get<bool>("readable");
        public bool Writable => JSRef.Get<bool>("writable");

        public JSEventCallback OnClose { get => new JSEventCallback("close", On, RemoveListener); set { } }

        public JSEventCallback OnInterested { get => new JSEventCallback("interested", On, RemoveListener); set { } }

        public JSEventCallback OnUninterested { get => new JSEventCallback("uninterested", On, RemoveListener); set { } }
        public JSEventCallback OnBitfield { get => new JSEventCallback("bitfield", On, RemoveListener); set { } }

        public JSEventCallback OnDownload { get => new JSEventCallback("download", On, RemoveListener); set { } }
        public JSEventCallback OnUpload{ get => new JSEventCallback("upload", On, RemoveListener); set { } }

        public JSEventCallback OnRequest { get => new JSEventCallback("request", On, RemoveListener); set { } }
        public JSEventCallback OnPort { get => new JSEventCallback("port", On, RemoveListener); set { } }

        public JSEventCallback OnHave { get => new JSEventCallback("have", On, RemoveListener); set { } }
        public JSEventCallback OnChoke { get => new JSEventCallback("choke", On, RemoveListener); set { } }
        public JSEventCallback OnUnchoke { get => new JSEventCallback("unchoke", On, RemoveListener); set { } }
        public JSEventCallback<string, string, JSObject> OnHandshake { get => new JSEventCallback<string, string, JSObject>("handshake", On, RemoveListener); set { } }
    }
}
