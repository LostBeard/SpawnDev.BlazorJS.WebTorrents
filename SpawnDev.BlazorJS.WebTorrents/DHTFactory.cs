namespace SpawnDev.BlazorJS.WebTorrents
{
    public class DHTFactory : IExtensionFactory
    {
        public string ExtensionName { get; } = "dht";
        Dictionary<string, DHT> DHTs = new Dictionary<string, DHT>();
        BlazorJSRuntime JS;
        public DHTFactory(BlazorJSRuntime js)
        {
            JS = js;
            Console.WriteLine($"DHTFactory()");
        }

        public Extension CreateExtension(Torrent torrent, Wire wire)
        {
            Console.WriteLine($"DHTFactory.CreateExtension()");
            var ret = new DHT(torrent, wire, ExtensionName);
            DHTs.Add(wire.InstanceId, ret);
            JS.Log($"ext_{wire.InstanceId}", ret);
            JS.Set($"ext_{wire.InstanceId}", ret);
            return ret;
        }
    }
}
