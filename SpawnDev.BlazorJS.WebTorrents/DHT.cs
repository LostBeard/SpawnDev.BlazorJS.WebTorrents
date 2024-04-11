namespace SpawnDev.BlazorJS.WebTorrents
{
    public class DHT : Extension
    {
        Torrent Torrent;
        public DHT(Torrent torrent, Wire wire, string extensionName) : base(wire, extensionName)
        {
            Torrent = torrent;
            Console.WriteLine($"new DHT()");
        }
    }
}
