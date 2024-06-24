using SpawnDev.BlazorJS.JSObjects;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// An instance of this class is created and returned by the WireExtensionFactory class for use with a new wire instance (if both sides support it)<br/>
    /// This object is serialized and passed to javascript. Only the below few properties are needed on that end.<br/>
    /// that is why the properties after have JsonIgnore.
    /// </summary>
    public class SimpleExtension : IDisposable
    {
        /// <summary>
        /// onHandshake?(infoHash: string, peerId: string, extensions: { [name: string]: boolean }): void;
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("onHandshake")]
        protected ActionCallback<string, string, Dictionary<string, bool>> onHandshake { get; }
        /// <summary>
        /// onExtendedHandshake?(handshake: { [key: string]: any }): void;
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("onExtendedHandshake")]
        protected ActionCallback<WireExtendedHandshakeEvent> onExtendedHandshake { get; }
        /// <summary>
        /// onMessage?(buf: Buffer): void;
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("onMessage")]
        protected ActionCallback<Uint8Array> onMessage { get; }
        /// <summary>
        /// Extension name<br/>
        /// name: string;
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string ExtensionName { get; private set; }
        // ******************************************************************************
        public delegate void MessageReceivedDelegate(SimpleExtension simpleExtension, Uint8Array msg);
        public event MessageReceivedDelegate OnMessageReceived;
        public delegate void HandshakeDelegate(SimpleExtension simpleExtension, string infoHash, string peerId, Dictionary<string, bool> extensions);
        public event HandshakeDelegate OnHandshake;
        public delegate void ExtendedHandshakeDelegate(SimpleExtension simpleExtension, WireExtendedHandshakeEvent extendedHandshake);
        public event ExtendedHandshakeDelegate OnExtendedHandshake;
        public delegate void CloseDelegate(SimpleExtension simpleExtension);
        public event CloseDelegate OnClose;
        /// <summary>
        /// The wire this extension instance is on
        /// </summary>
        [JsonIgnore]
        public Wire Wire { get; private set; }
        /// <summary>
        /// The torrent this extension instance is on
        /// </summary>
        [JsonIgnore]
        public Torrent Torrent { get; private set; }
        /// <summary>
        /// Returns true if the peer reported supporting this extension name
        /// </summary>
        [JsonIgnore]
        public bool SupportedPeer { get; private set; }
        /// <summary>
        /// Set to the infoHash of this torrent once it is known
        /// </summary>
        [JsonIgnore]
        public string? InfoHash { get; private set; }
        /// <summary>
        /// The peer id this wire is connected to
        /// </summary>
        [JsonIgnore]
        public string PeerId { get; private set; } 
        /// <summary>
        /// Data received during the extended handshake
        /// </summary>
        [JsonIgnore]
        public WireExtendedHandshakeEvent? ExtendedHandshake { get; private set; } = null;
        /// <summary>
        /// Used to encode and decode BEncoded data
        /// </summary>
        public SimpleExtension(Torrent torrent, Wire wire, string extensionName)
        {
            ExtensionName = extensionName;
            onHandshake = new ActionCallback<string, string, Dictionary<string, bool>>(_OnHandshake);
            onExtendedHandshake = new ActionCallback<WireExtendedHandshakeEvent>(_OnExtendedHandshake);
            onMessage = new ActionCallback<Uint8Array>(_OnMessage);
            Wire = wire.JSRefCopy<Wire>();
            Torrent = torrent.JSRefCopy<Torrent>();
            PeerId = Wire.PeerId;
            InfoHash = torrent.InfoHash;
            Wire.OnClose += Wire_OnClose;
        }
        void Wire_OnClose()
        {
            OnClose?.Invoke(this);
        }
        /// <summary>
        /// byte arrays and Uint8Array data will be sent, without change, as a Uint8Array<br />
        /// all other data is BEncoded before being sent
        /// </summary>
        /// <param name="data"></param>
        /// <param name="extensionName">If specified, this is the target extension on the remote peer the message is for</param>
        /// <returns></returns>
        public virtual bool Send(object data, string? extensionName = null)
        {
            if (Wire == null || (!SupportedPeer && string.IsNullOrEmpty(extensionName))) return false;
            var destExt = string.IsNullOrEmpty(extensionName) ? ExtensionName : extensionName;
            try
            {
                Wire.Extended(destExt, data);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wire.Extended error: {ex.Message}");
            }
            return false;
        }
        void _OnHandshake(string infoHash, string peerId, Dictionary<string, bool> extensions)
        {
            InfoHash = infoHash;
            OnHandshake?.Invoke(this, infoHash, peerId, extensions);
        }
        void _OnExtendedHandshake(WireExtendedHandshakeEvent extendedHandshake)
        {
            ExtendedHandshake = extendedHandshake;
            var m = extendedHandshake.M;
            SupportedPeer = m != null && m.ContainsKey(ExtensionName);
            OnExtendedHandshake?.Invoke(this, extendedHandshake);
        }
        /// <summary>
        /// If using BencodeNet, the below line will decode text<br/>
        /// var txt = BencodeParser.Parse(buf).ToString();
        /// </summary>
        /// <param name="buf"></param>
        void _OnMessage(Uint8Array buf)
        {
            OnMessageReceived?.Invoke(this, buf);
        }
        /// <summary>
        /// Returns true if this instance has been disposed
        /// </summary>
        [JsonIgnore]
        public bool IsDisposed { get; private set; }
        /// <summary>
        /// Releases resources
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Wire.Dispose();
            Torrent.Dispose();
            Wire.OnClose -= Wire_OnClose;
            onHandshake.Dispose(); 
            onExtendedHandshake.Dispose(); 
            onMessage.Dispose();
        }
    }
}
