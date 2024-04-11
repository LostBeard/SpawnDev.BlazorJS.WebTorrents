using System.Diagnostics.CodeAnalysis;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class DHTFactory : IExtensionFactory
    {
        public string ExtensionName { get; } = "dht";
        Dictionary<string, DHT> DHTs = new Dictionary<string, DHT>();
        public Extension CreateExtension(Torrent torrent, Wire wire)
        {
            Console.WriteLine($"DHTFactory.CreateExtension()");
            var ret = new DHT(torrent, wire, ExtensionName);
            DHTs.Add(wire.InstanceId, ret);
            return ret;
        }
    }
}
