using BencodeNET.Torrents;
using Radzen;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Shared
{
    public class TorrentsDataGridItem
    {
        public ProgressBarStyle ProgressBarStyle
        {
            get
            {
                return Torrent.Done ? ProgressBarStyle.Success : (
                    Torrent.NumPeers == 0 ? ProgressBarStyle.Danger : (
                        Torrent.DownloadSpeed == 0 ? ProgressBarStyle.Warning : ProgressBarStyle.Info
                    )
                );
            }
        }
        public int TrackerSeeders => Torrent.Announced?.Values.Where(o => !o.Expired).Sum(o => o.Complete) ?? 0;
        public int TrackerPeers => Torrent.Announced?.Values.Where(o => !o.Expired).Sum(o => o.Incomplete) ?? 0;
        public double UploadSpeed => Torrent.UploadSpeed;
        public double TimeRemaining => Torrent.TimeRemaining ?? TimeSpan.MaxValue.TotalSeconds + 1d;
        public double DownloadSpeed => Torrent.DownloadSpeed;
        public double Length => Torrent.Length;
        public double Downloaded => Torrent.Downloaded;
        public double Progress => Torrent.Progress;
        public string Name => Torrent.Name;
        public string InstanceId => Torrent.InstanceId;
        public string InfoHash => Torrent.InfoHash;
        public JSObjects.Array<Wire> Wires;
        public int Seeds
        {
            get
            {
                var wires = Wires.ToArray();
                var ret = wires.Where(o => o.IsSeeder).Count();
                wires.DisposeAll();
                return ret;
            }
        }
        public int Peers
        {
            get
            {
                var wires = Wires.ToArray();
                var ret = wires.Where(o => !o.IsSeeder).Count();
                wires.DisposeAll();
                return ret;
            }
        }
        public Torrent Torrent { get; }
        public TorrentsDataGridItem(Torrent torrent)
        {
            Torrent = torrent;
            Wires = torrent.Wires;
        }
    }
}
