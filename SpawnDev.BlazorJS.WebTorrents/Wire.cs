using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;
using System.Security.Cryptography;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://github.com/webtorrent/bittorrent-protocol !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// <summary>
    /// WebTorrent Wire class<br />
    /// https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#wire-api
    /// </summary>
    public class Wire : EventEmitter
    {
        /// <summary>
        /// Returns the property instanceId, setting to a new value if not set
        /// </summary>
        public string InstanceId
        {
            get
            {
                var value = JSRef!.Get<string?>("instanceId");
                if (string.IsNullOrEmpty(value))
                {
                    value = $"WIRE_{Convert.ToHexString(RandomNumberGenerator.GetBytes(16))}";
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
        public double UploadSpeed() => JSRef.Call<double>("uploadSpeed");
        /// <summary>
        /// Peer download speed, in bytes/sec.
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
        /// <summary>
        /// ExtendedHandshake properties can be set when an extension is created and those properties will be sent to peers when an extendedHandshake occurs
        /// </summary>
        public ExtendedHandshake ExtendedHandshake => JSRef.Get<ExtendedHandshake>("extendedHandshake");
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
        /// Returns true if the torrent stroe is readable
        /// </summary>
        public bool Readable => JSRef.Get<bool>("readable");
        /// <summary>
        /// Returns true if the torrent store is writable
        /// </summary>
        public bool Writable => JSRef.Get<bool>("writable");
        // Events
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
