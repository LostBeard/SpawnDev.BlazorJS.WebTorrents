using Microsoft.JSInterop;
using SpawnDev.BlazorJS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class WebrtcConn : JSObject
    {
        public string ChannelName => JSRef.Get<string>("channelName");
        public bool Destroying => JSRef.Get<bool>("destroying");
        public string Id => JSRef.Get<string>("id");
        public bool Initiator => JSRef.Get<bool>("initiator");
        public string LocalAddress => JSRef.Get<string>("localAddress");
        public string LocalFamily => JSRef.Get<string>("localFamily");
        public int LocalPort => JSRef.Get<int>("localPort");
        public bool Readable => JSRef.Get<bool>("readable");
        public string RemoteAddress => JSRef.Get<string>("remoteAddress");
        public string RemoteFamily => JSRef.Get<string>("remoteFamily");
        public int RemotePort => JSRef.Get<int>("remotePort");
        public bool Writable => JSRef.Get<bool>("writable");
        public WebrtcConn(IJSInProcessObjectReference _ref) : base(_ref) { }
    }
}
