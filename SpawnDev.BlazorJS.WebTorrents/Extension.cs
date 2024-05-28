﻿using BencodeNET.Parsing;
using System.Text.Json.Serialization;
using Timer = System.Timers.Timer;

namespace SpawnDev.BlazorJS.WebTorrents
{

    /// <summary>
    /// An instance of this class is created and returned by the WireExtensionFactory class for use with a new wire instance (if both sides support it)
    /// </summary>
    public abstract class Extension : IDisposable
    {
        // This object is serialized and passed to javascript. Only the below few properties are needed on that end.
        // that is why the properties after have JsonIgnore.
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
        protected ActionCallback<byte[]> onMessage { get; }
        /// <summary>
        /// name: string;
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name { get; private set; }
        // ******************************************************************************
        public delegate void MessageReceivedDelegate(Extension extension, byte[] msg);
        public event MessageReceivedDelegate OnMessageReceived;
        public event Action<Extension> OnClose;
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
        /// <summary>
        /// Used to encode and decode BEncoded data
        /// </summary>
        protected BencodeParser BencodeParser = new BencodeParser();
        public Extension(Wire wire, string extensionName)
        {
            JS = BlazorJSRuntime.JS;
            Name = extensionName;
            onHandshake = _callbacks.Add(new ActionCallback<string, string, Dictionary<string, bool>>(_OnHandshake));
            onExtendedHandshake = _callbacks.Add(new ActionCallback<WireExtendedHandshakeEvent>(_OnExtendedHandshake));
            onMessage = _callbacks.Add(new ActionCallback<byte[]>(_OnMessage));
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
            //Send($"........");
        }

        /// <summary>
        /// byte arrays and Uint8Array data will be sent, without change, as a Uint8Array<br />
        /// all other data is BEncoded before being sent
        /// </summary>
        /// <param name="data"></param>
        /// <param name="extensionName">If specified, this is the target extension on the remote peer the message is for</param>
        /// <returns></returns>
        protected virtual bool Send(object data, string? extensionName = null)
        {
            if (Wire == null || !SupportedPeer) return false;
            var destExt = string.IsNullOrEmpty(extensionName) ? Name : extensionName;
            try
            {
                JS.Log(Name, "Send", destExt, data);
                Wire.Extended(destExt, data);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("wire ping error");
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        protected virtual void _OnHandshake(string infoHash, string peerId, Dictionary<string, bool> extensions)
        {
            InfoHash = infoHash;
            JS.Log(Name, "_OnHandshake", infoHash, peerId, extensions);
        }

        public delegate void SupportedPeerConnectedDelegate(Extension wireExtension, WireExtendedHandshakeEvent extendedHandshake);
        public event SupportedPeerConnectedDelegate OnSupportedPeerConnected;

        protected virtual void _OnExtendedHandshake(WireExtendedHandshakeEvent extendedHandshake)
        {
            //JS.Log(Name, "_OnExtendedHandshake !!!!!!!!!!!!!!!:", extendedHandshake.Extensions, extendedHandshake);
            ExtendedHandshake = extendedHandshake;
            var m = extendedHandshake.M;
            SupportedPeer = m != null && m.ContainsKey(Name);
            JS.Log(Name, "_OnExtendedHandshake: supportsExtension", SupportedPeer, extendedHandshake);
            if (SupportedPeer)
            {
                OnSupportedPeerConnected?.Invoke(this, extendedHandshake);
                //JS.Log($"SupportedPeer: {SupportedPeer}");
                //Send($"Hello World Ext > {Name}");
            }
            else
            {

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
                var txt = BencodeParser.Parse(buf).ToString();
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
}