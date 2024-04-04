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
        public int TotalSeeders => (Torrent.Announced?.Values.Where(o => !o.Expired()).Sum(o => o.Complete) ?? 0) + WebSeeds;
        public int TotalPeers => Torrent.Announced?.Values.Where(o => !o.Expired()).Sum(o => o.Incomplete) ?? 0;
        public int WebSeeds
        {
            get
            {
                using var wires = Torrent.Wires;
                var w = wires.ToArray();
                var ret = w.Where(o => o.Type == "webSeed").Count();
                w.DisposeAll();
                return ret;
            }
        }
        public double UploadSpeed => Torrent.UploadSpeed;
        public double TimeRemaining => Torrent.TimeRemaining ?? TimeSpan.MaxValue.TotalSeconds + 1d;
        public double DownloadSpeed => Torrent.DownloadSpeed;
        public double Length => Torrent.Length;
        public double Downloaded => Torrent.Downloaded;
        public int TotalPieces => Torrent.Pieces.Length;
        public int SelectedPieces => Torrent.Selections.Length;
        public double Progress => Torrent.Progress;
        public string Name => Torrent.Name;
        public string InstanceId => Torrent.InstanceId;
        public string InfoHash => Torrent.InfoHash;
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
        public JSObjects.Array<Wire> Wires { get; }
        public TorrentsDataGridItem(Torrent torrent)
        {
            Torrent = torrent;
            Wires = torrent.Wires;
        }
    }
}
