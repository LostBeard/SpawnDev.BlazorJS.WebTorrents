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

    public class WebConn : JSObject
    {
        public string PeerId => JSRef.Get<string>("peerId");
        public string Url => JSRef.Get<string>("url");
        public string WebPeerId => JSRef.Get<string>("webPeerId");
        public bool Readable => JSRef.Get<bool>("readable");
        public bool Writable => JSRef.Get<bool>("writable");
        public WebConn(IJSInProcessObjectReference _ref) : base(_ref) { }
    }
}
