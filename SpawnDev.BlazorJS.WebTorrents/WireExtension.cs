using BencodeNET.Parsing;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Timer = System.Timers.Timer;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // An instance of this class is created and returned by the WireExtensionFactory class for use with a new wire instance (if both sides support it)
    public class WireExtension : IDisposable
    {
        // This object is serialized and passed to javascript. Only the below few properties are needed on that end. that is why the properties after have JsonIgnore.
        public ActionCallback<string, string, JSObject> OnHandshake { get;  }
        public ActionCallback<WireExtendedHandshakeEvent> OnExtendedHandshake { get;  }
        public ActionCallback<byte[]> OnMessage { get; }
        public string Name { get; set; }
        // ******************************************************************************
        public delegate void MessageReceivedDelegate(WireExtension extension, byte[] msg);
        public event MessageReceivedDelegate OnMessageReceived;
        protected CallbackGroup _callbacks = new CallbackGroup();
        BlazorJSRuntime JS;
        Timer _tmr = new Timer();
        [JsonIgnore]
        public Wire Wire { get; private set; }
        [JsonIgnore]
        public bool SupportedPeer { get; private set; }
        [JsonIgnore]
        public string InfoHash { get; private set; } = "";
        [JsonIgnore]
        public WireExtendedHandshakeEvent? ExtendedHandshake { get; private set; } = null;
        public WireExtension(Wire wire, string extensionName)
        {
            JS = BlazorJSRuntime.JS;
            Name = extensionName;
            OnHandshake = _callbacks.Add(new ActionCallback<string, string, JSObject>(_OnHandshake));
            OnExtendedHandshake = _callbacks.Add(new ActionCallback<WireExtendedHandshakeEvent>(_OnExtendedHandshake));
            OnMessage = _callbacks.Add(new ActionCallback<byte[]>(_OnMessage));
            Wire = JS.ReturnMe(wire);
            _tmr.Elapsed += _tmr_Elapsed;
            _tmr.Interval = 5000;
            _tmr.Enabled = true;
        }

        private void _tmr_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Send($"........");
        }

        public bool Send(object data)
        {
            if (Wire == null || !SupportedPeer) return false;
            try
            {
                Wire.Extended(Name, data);
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("wire ping error");
                //Console.WriteLine(ex.Message);
            }
            return false;
        }

        void _OnHandshake(string infoHash, string peerId, JSObject extensions)
        {
            InfoHash = infoHash;
            JS.Log(Name, "_OnHandshake", infoHash, peerId, extensions);
            extensions.Dispose();
        }

        public delegate void SupportedPeerConnectedDelegate(WireExtension wireExtension, WireExtendedHandshakeEvent extendedHandshake);
        public event SupportedPeerConnectedDelegate OnSupportedPeerConnected;

        void _OnExtendedHandshake(WireExtendedHandshakeEvent extendedHandshake)
        {
            ExtendedHandshake = extendedHandshake;
            var m = extendedHandshake.M;
            SupportedPeer = m != null && m.ContainsKey(Name);
            JS.Log(Name, "OnExtendedHandshake: supportsExtension", SupportedPeer, extendedHandshake);
            if (SupportedPeer)
            {
                OnSupportedPeerConnected?.Invoke(this, extendedHandshake);
                Send($"Hello World Ext > {Name}");
            }
        }

        // https://github.com/Krusen/BencodeNET#encoding
        BencodeParser parser = new BencodeParser();
        void _OnMessage(byte[] buf)
        {
            var txt = parser.Parse(buf).ToString();
            JS.Log(Name, "_OnMessage", txt);
            OnMessageReceived?.Invoke(this, buf);
        }

        public void Dispose()
        {
            ExtendedHandshake?.Dispose();
            ExtendedHandshake = null;
            _callbacks.Dispose();
        }
    }
    // Not exactly positive for the below class.
    // Could not find and actual API documentation for wire extensions, just the documented tip to look at the ut_metadata source code as reference.
    public class WireExtendedHandshakeEvent : ExtendedHandshake
    {
        public WireExtendedHandshakeEvent(IJSInProcessObjectReference _ref) : base(_ref) { }

        public Dictionary<string, int>? M => JSRef.Get<Dictionary<string, int>>("m");
    }
    // Not exactly positive for the below class.
    // Could not find and actual API documentation for wire extensions, just the documented tip to look at the ut_metadata source code as reference.
    // Tried implementing it in a mostly generic way
    public class ExtendedHandshake : JSObject
    {
        public ExtendedHandshake(IJSInProcessObjectReference _ref) : base(_ref) { }
        public void SetItem(string key, int value) => JSRef.Set(key, value);
        public T GetItem<T>(string key) => JSRef.Get<T>(key);
    }
}
