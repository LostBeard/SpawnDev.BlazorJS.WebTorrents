using SpawnDev.BlazorJS.JSObjects;
using static System.Net.Mime.MediaTypeNames;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class DHT : Extension
    {
        Torrent Torrent;
        BlazorJSRuntime JS;
        public DHT(BlazorJSRuntime js, Torrent torrent, Wire wire, string extensionName) : base(wire, extensionName)
        {
            JS = js;
            Torrent = torrent;
            Console.WriteLine($"new DHT(): {wire.PeerId}");
            wire.OnHandshake += Wire_OnHandshake;
            wire.OnExtended += Wire_OnExtended;
        }
        void Wire_OnExtended(string ext, JSObject buf)
        {
            JS.Log("Wire_OnExtended",  ext, buf);
        }
        void Wire_OnHandshake(string infoHash, string peerId, Array<JSObject> extensions /* Extension[] */)
        {
            JS.Log("Wire_OnHandshake", infoHash, peerId, extensions);
        }
    }
}
