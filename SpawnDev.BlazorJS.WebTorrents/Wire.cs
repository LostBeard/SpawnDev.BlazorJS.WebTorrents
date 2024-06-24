using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;
using System.Security.Cryptography;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// WebTorrent Wire class<br />
    /// node_modules\@types\bittorrent-protocol\index.d.ts<br />
    /// node_modules\@types\node\stream.d.ts<br />
    /// https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#wire-api<br />
    /// </summary>
    public class Wire : Stream
    {
        /// <summary>
        /// Returns the property instanceId, setting to a new value if not set<br />
        /// non-standard property
        /// </summary>
        public string InstanceId
        {
            get
            {
                var value = JSRef!.Get<string?>("instanceId");
                if (string.IsNullOrEmpty(value))
                {
                    value = $"{GetType().Name}_{Convert.ToHexString(RandomNumberGenerator.GetBytes(16))}";
                    JSRef!.Set("instanceId", value);
                }
                return value;
            }
        }
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Wire(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Remote peer id (hex string)
        /// </summary>
        public string PeerId => JSRef.Get<string>("peerId");
        /// <summary>
        /// Remote address or null if not specified. Not available with some peer types.
        /// </summary>
        public string? RemoteAddress => JSRef.Get<string?>("remoteAddress");
        /// <summary>
        /// Remote port or null if not specified. Not available with some peer types.
        /// </summary>
        public ushort? RemotePort => JSRef.Get<ushort?>("remotePort");
        /// <summary>
        /// The peers peerId as a Uint8Array
        /// </summary>
        public Uint8Array PeerIdBuffer => JSRef.Get<Uint8Array>("peerId");
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
        /// PeerPieces instance
        /// </summary>
        public Bitfield PeerPieces => JSRef.Get<Bitfield>("peerPieces");
        /// <summary>
        /// Returns the extended handshake as a JSObject
        /// </summary>
        public JSObject ExtendedHandshake => JSRef.Get<JSObject>("extendedHandshake");
        /// <summary>
        /// The extended handshake from the peer
        /// </summary>
        public JSObject PeerExtendedHandshake => JSRef.Get<JSObject>("peerExtendedHandshake");
        /// <summary>
        /// Upload speed to peer, in bytes/sec.
        /// </summary>
        /// <returns></returns>
        public double UploadSpeed() => JSRef.Call<double>("uploadSpeed");
        /// <summary>
        /// Download speed from peer, in bytes/sec.
        /// </summary>
        public double DownloadSpeed() => JSRef.Call<double>("downloadSpeed");
        /// <summary>
        /// Close the connection with the peer. This however doesn't prevent the peer from simply re-connecting.
        /// </summary>
        public void Destroy() => JSRef.CallVoid("destroy");
        /// <summary>
        /// Tell the wire to use the given extension factory<br />
        /// An "extension factory" 
        /// </summary>
        /// <param name="extensionConstructor"></param>
        /// <param name="extensionName"></param>
        public void Use(Function extensionConstructor, string? extensionName = null)
        {
            // the "use" method checks for extension.prototype.name
            if (!string.IsNullOrEmpty(extensionName)) extensionConstructor.JSRef!.Set("prototype.name", extensionName);
            JSRef!.CallVoid("use", extensionConstructor);
        }
        /// <summary>
        /// Tell the wire to use Extension constructor
        /// </summary>
        /// <param name="extensionName"></param>
        /// <param name="extensionConstructor"></param>
        public void Use(string extensionName, Func<Wire, Extension> extensionConstructor)
        {
            // the "use" method checks for extension.prototype.name
            var callback = new FuncCallback<Wire, Extension>(extensionConstructor, true);
            using var callbackFN = JS.ReturnMe<Function>(callback)!;
            callbackFN.JSRef!.Set("prototype.name", extensionName);
            JSRef!.CallVoid("use", callback);
        }
        public void Use(string extensionName, Func<Wire, SimpleExtension> extensionConstructor)
        {
            // the "use" method checks for extension.prototype.name
            var callback = new FuncCallback<Wire, SimpleExtension>(extensionConstructor, true);
            using var callbackFN = JS.ReturnMe<Function>(callback)!;
            callbackFN.JSRef!.Set("prototype.name", extensionName);
            JSRef!.CallVoid("use", callback);
        }
        public void Use(string extensionName, Func<SimpleExtension> extensionConstructor)
        {
            // the "use" method checks for extension.prototype.name
            var callback = new FuncCallback<SimpleExtension>(extensionConstructor, true);
            using var callbackFN = JS.ReturnMe<Function>(callback)!;
            callbackFN.JSRef!.Set("prototype.name", extensionName);
            JSRef!.CallVoid("use", callback);
        }
        /// <summary>
        /// Tell the wire to use Extension constructor
        /// </summary>
        /// <param name="torrent"></param>
        /// <param name="extensionName"></param>
        /// <param name="extensionConstructor"></param>
        public void Use(Torrent torrent, string extensionName, Func<Torrent, Wire, Extension> extensionConstructor)
        {
            var torrentCopy = torrent.JSRefCopy<Torrent>();
            Use(extensionName, (wire) => extensionConstructor(torrentCopy, wire));
        }
        /// <summary>
        /// Creates an instance of SimpleExtension, attaches it to the wire and returns it.
        /// </summary>
        /// <param name="torrent"></param>
        /// <param name="extensionName"></param>
        /// <returns></returns>
        public SimpleExtension Use(Torrent torrent, string extensionName)
        {
            var simpleExtension = new SimpleExtension(torrent, this, extensionName);
            Use(extensionName, () => simpleExtension);
            return simpleExtension;
        }
        /// <summary>
        /// Send data to the named extension on the other end of the wire<br />
        /// byte arrays and Uint8Array data will be sent, without change, as a Uint8Array<br />
        /// all other data is BEncoded before being sent
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="data"></param>
        public void Extended(string extension, object data) => JSRef.CallVoid("extended", extension, data);
        ///// <summary>
        ///// ExtendedHandshake properties can be set when an extension is created and those properties will be sent to peers when an extendedHandshake occurs
        ///// </summary>
        //public ExtendedHandshake ExtendedHandshake => JSRef.Get<ExtendedHandshake>("extendedHandshake");
        /// <summary>
        /// Returns true if choking the peer
        /// </summary>
        public bool AmChoking => JSRef.Get<bool>("amChoking");
        /// <summary>
        /// Returns true if interested in pieces the peer has
        /// </summary>
        public bool AmInterested => JSRef.Get<bool>("amInterested");
        /// <summary>
        /// Returns true if the peer is a seeder
        /// </summary>
        public bool IsSeeder => JSRef.Get<bool>("isSeeder");
        /// <summary>
        /// Returns true if the peer is choking us
        /// </summary>
        public bool PeerChoking => JSRef.Get<bool>("peerChoking");
        /// <summary>
        /// Returns true if the peer is interested in pieces we have
        /// </summary>
        public bool PeerInterested => JSRef.Get<bool>("peerInterested");
        /// <summary>
        /// Requests
        /// </summary>
        public Array<Request> Requests => JSRef.Get<Array<Request>>("requests");
        /// <summary>
        /// Peer requests
        /// </summary>
        public Array<Request> PeerRequests => JSRef.Get<Array<Request>>("peerRequests");
        /// <summary>
        /// ExtendedMapping
        /// </summary>
        public Dictionary<int,string> ExtendedMapping => JSRef.Get<Dictionary<int, string>>("extendedMapping");
        /// <summary>
        /// PeerExtendedMapping
        /// </summary>
        public Dictionary<string, int> PeerExtendedMapping => JSRef.Get<Dictionary<string, int>>("peerExtendedMapping");
        /// <summary>
        /// Returns true if the torrent stroe is readable
        /// </summary>
        public bool Readable => JSRef.Get<bool>("readable");
        /// <summary>
        /// Returns true if the torrent store is writable
        /// </summary>
        public bool Writable => JSRef.Get<bool>("writable");
        /// <summary>
        /// Returns supported extensions
        /// </summary>
        public Dictionary<string, bool> Extensions => JSRef.Get<Dictionary<string, bool>>("extensions");
        /// <summary>
        /// Returns peer supported extensions
        /// </summary>
        public Dictionary<string, bool> PeerExtensions => JSRef.Get<Dictionary<string, bool>>("peerExtensions");
        // Events
        /// <summary>
        /// Emitted on error
        /// </summary>
        public JSEventCallback OnError { get => new JSEventCallback("error", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when a bitfield is received<br />
        /// bitfield: bitfield instance
        /// </summary>
        public JSEventCallback<Bitfield> OnBitfield { get => new JSEventCallback<Bitfield>("bitfield", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted whe na keep alive message is received
        /// </summary>
        public JSEventCallback OnKeepAlive { get => new JSEventCallback("keep-alive", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted on timeout
        /// </summary>
        public JSEventCallback OnTimeout { get => new JSEventCallback("timeout", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when connection choked
        /// </summary>
        public JSEventCallback OnChoke { get => new JSEventCallback("choke", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when connection unchoked
        /// </summary>
        public JSEventCallback OnUnchoke { get => new JSEventCallback("unchoke", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when interested message received
        /// </summary>
        public JSEventCallback OnInterested { get => new JSEventCallback("interested", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when uninterested message received
        /// </summary>
        public JSEventCallback OnUninterested { get => new JSEventCallback("uninterested", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted on upload<br />
        /// length: int
        /// </summary>
        public JSEventCallback<int> OnUpload { get => new JSEventCallback<int>("upload", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted on have message<br />
        /// index: int
        /// </summary>
        public JSEventCallback<int> OnHave { get => new JSEventCallback<int>("have", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted on download<br />
        /// length: int
        /// </summary>
        public JSEventCallback<int> OnDownload { get => new JSEventCallback<int>("download", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted on handshake<br />
        /// infoHash: string<br />
        /// peerId: string<br />
        /// extensions: Extension[] (TODO - verify)
        /// </summary>
        public JSEventCallback<string, string, Array<JSObject>> OnHandshake { get => new JSEventCallback<string, string, Array<JSObject>>("handshake", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted on piece request received<br />
        /// index: number<br />
        /// offset: number<br />
        /// length: number<br />
        /// respond: () => void
        /// </summary>
        public JSEventCallback<int, int, int, Function> OnRequest { get => new JSEventCallback<int, int, int, Function>("request", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when a piece is received<br />
        /// index: number<br />
        /// offset: number<br />
        /// buffer: Buffer
        /// </summary>
        public JSEventCallback<int, int, Uint8Array> OnPiece { get => new JSEventCallback<int, int, Uint8Array>("piece", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when a piece request is cancelled
        /// </summary>
        public JSEventCallback<int, int, int> OnCancel { get => new JSEventCallback<int, int, int>("cancel", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted on extended<br />
        /// ext: "handshake" | string<br />
        /// buf: any
        /// </summary>
        public JSEventCallback<string, JSObject> OnExtended { get => new JSEventCallback<string, JSObject>("extended", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted on unknown message received<br />
        /// buffer: Buffer
        /// </summary>
        public JSEventCallback<JSObject> OnUnknownMessage { get => new JSEventCallback<JSObject>("unknownmessage", On, RemoveListener); set { } }
    }
}
