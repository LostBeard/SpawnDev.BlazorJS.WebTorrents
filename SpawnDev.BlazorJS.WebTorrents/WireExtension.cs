using BencodeNET.Parsing;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;
using Timer = System.Timers.Timer;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // An instance of this class is created and returned by the WireExtensionFactory class for use with a new wire instance (if both sides support it)
    public class WireExtension : IDisposable
    {
        // This object is serialized and passed to javascript. Only the below few properties are needed on that end.
        // that is why the properties after have JsonIgnore.
        [JsonInclude]
        [JsonPropertyName("onHandshake")]
        public ActionCallback<string, string, JSObject> OnHandshake { get; }
        [JsonInclude]
        [JsonPropertyName("onExtendedHandshake")]
        public ActionCallback<WireExtendedHandshakeEvent> OnExtendedHandshake { get; }
        [JsonInclude]
        [JsonPropertyName("onMessage")]
        public ActionCallback<byte[]> OnMessage { get; }
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        // ******************************************************************************
        public delegate void MessageReceivedDelegate(WireExtension extension, byte[] msg);
        public event MessageReceivedDelegate OnMessageReceived;

        public event Action<WireExtension> OnClose;

        protected CallbackGroup _callbacks = new CallbackGroup();
        protected BlazorJSRuntime JS;
        protected Timer _tmr = new Timer();
        [JsonIgnore]
        public Wire Wire { get; private set; }
        [JsonIgnore]
        public bool SupportedPeer { get; private set; }
        [JsonIgnore]
        public string InfoHash { get; private set; } = "";
        [JsonIgnore]
        public string PeerId { get; private set; } = "";
        [JsonIgnore]
        public WireExtendedHandshakeEvent? ExtendedHandshake { get; private set; } = null;

        // https://github.com/Krusen/BencodeNET#encoding
        BencodeParser parser = new BencodeParser();
        public WireExtension(Wire wire, string extensionName)
        {
            JS = BlazorJSRuntime.JS;
            Name = extensionName;
            OnHandshake = _callbacks.Add(new ActionCallback<string, string, JSObject>(_OnHandshake));
            OnExtendedHandshake = _callbacks.Add(new ActionCallback<WireExtendedHandshakeEvent>(_OnExtendedHandshake));
            OnMessage = _callbacks.Add(new ActionCallback<byte[]>(_OnMessage));
            Wire = JS.ReturnMe(wire);
            PeerId = Wire.PeerId;
            Wire.OnClose += Wire_OnClose;
            _tmr.Elapsed += _tmr_Elapsed;
            _tmr.Interval = 5000;
            _tmr.Enabled = true;
        }

        protected virtual void Wire_OnClose()
        {
            JS.Log($"Wire_OnClose", Wire.PeerId);
            OnClose?.Invoke(this);
        }

        private void _tmr_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Send($"........");
        }

        /// <summary>
        /// byte arrays and Uint8Array data will be sent, without change, as a Uint8Array<br />
        /// all other data is BEncoded before being sent
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual bool Send(object data) => Send(Name, data);
        protected virtual bool Send(string toExtensionName, object data)
        {
            if (Wire == null || !SupportedPeer) return false;
            try
            {
                Wire.Extended(toExtensionName, data);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("wire ping error");
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        protected virtual void _OnHandshake(string infoHash, string peerId, JSObject extensions)
        {
            InfoHash = infoHash;
            JS.Log(Name, "_OnHandshake", infoHash, peerId, extensions);
            extensions.Dispose();
        }

        public delegate void SupportedPeerConnectedDelegate(WireExtension wireExtension, WireExtendedHandshakeEvent extendedHandshake);
        public event SupportedPeerConnectedDelegate OnSupportedPeerConnected;

        protected virtual void _OnExtendedHandshake(WireExtendedHandshakeEvent extendedHandshake)
        {
            JS.Log(Name, "OnExtendedHandshake !!!!!!!!!!!!!!!:", extendedHandshake.Extensions, extendedHandshake);
            ExtendedHandshake = extendedHandshake;
            var m = extendedHandshake.M;
            SupportedPeer = m != null && m.ContainsKey(Name);
            //JS.Log(Name, "OnExtendedHandshake 1: supportsExtension", SupportedPeer, extendedHandshake);
            if (SupportedPeer)
            {
                OnSupportedPeerConnected?.Invoke(this, extendedHandshake);
                Send($"Hello World Ext > {Name}");
            }
        }

        /// <summary>
        /// This method will be called by the web torrent instance when there is a message for this extension
        /// </summary>
        /// <param name="buf"></param>
        void _OnMessage(byte[] buf)
        {
            try
            {
                var txt = parser.Parse(buf).ToString();
                JS.Log(Name, "_OnMessage", txt);
            }
            catch (Exception ex)
            {
                JS.Log("WARNING: Message is not bencoded.");
            }
            OnMessageReceived?.Invoke(this, buf);
        }

        public void Dispose()
        {
            Wire.OnClose -= Wire_OnClose;
            _callbacks.Dispose();
        }
    }
    // Not exactly positive for the below class.
    // Could not find an actual API documentation for wire extensions, just the documented tip to look at the ut_metadata source code as reference.
    public class WireExtendedHandshakeEvent : ExtendedHandshake
    {
        public WireExtendedHandshakeEvent(IJSInProcessObjectReference _ref) : base(_ref) { }

        /// <summary>
        /// m will contain a dictionary where the keys being the supported wire extension names<br />
        /// Dictionary of supported extension messages which maps names of extensions to an extended message ID for each extension message. The only requirement on these IDs is that no extension message share the same one. Setting an extension number to zero means that the extension is not supported/disabled. The client should ignore any extension names it doesn't recognize.
        /// </summary>
        [JsonPropertyName("m")]
        public Dictionary<string, int>? M => JSRef.Get<Dictionary<string, int>>("m");

        public List<string> Extensions => JSRef.Get<JSObject>("m").JSRef!.GetPropertyNames();
    }
    // Not exactly positive for the below class.
    // Could not find an actual API documentation for wire extensions, just the documented tip to look at the ut_metadata source code as reference.
    // Tried implementing it in a mostly generic way
    public class ExtendedHandshake : JSObject
    {
        public ExtendedHandshake(IJSInProcessObjectReference _ref) : base(_ref) { }
        public void SetItem(string key, int value) => JSRef.Set(key, value);
        public T GetItem<T>(string key) => JSRef.Get<T>(key);
    }
}
