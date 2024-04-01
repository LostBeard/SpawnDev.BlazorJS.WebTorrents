using BencodeNET.Torrents;
using Radzen;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Shared
{
    public class TorrentStateInfo
    {
        public ProgressBarStyle ProgressBarStyle
        {
            get
            {
                return Torrent.Done ? ProgressBarStyle.Success : (
                    Torrent.WireCount == 0 ? ProgressBarStyle.Danger : (
                        Torrent.DownloadSpeed == 0 ? ProgressBarStyle.Warning : ProgressBarStyle.Info
                    )
                );
            }
        }
        public double UploadSpeed => Torrent.UploadSpeed;
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
        public TorrentStateInfo(Torrent torrent)
        {
            Torrent = torrent;
            Wires = torrent.Wires;
        }
    }
    public class TorrentFileStateInfo
    {
        public ProgressBarStyle ProgressBarStyle
        {
            get
            {
                if (File == null || Torrent == null)
                {
                    return ProgressBarStyle.Danger;
                }
                return File.IsDone() ? ProgressBarStyle.Success : (
                Torrent.WireCount == 0 ? ProgressBarStyle.Danger : (
                    Torrent.DownloadSpeed == 0 ? ProgressBarStyle.Warning : ProgressBarStyle.Info
                )
            );
            }
        }
        public double Length => File.Length;
        public double Downloaded => File.Downloaded;
        public double Progress => File.Progress;
        public string Name => File.Name;
        public JSObjects.Array<Wire> Wires;
        public JSObjects.Array<File> Files;
        //public int Seeds
        //{
        //    get
        //    {
        //        var wires = Wires.ToArray();
        //        var ret = wires.Where(o => o.IsSeeder).Count();
        //        wires.DisposeAll();
        //        return ret;
        //    }
        //}
        //public int Peers
        //{
        //    get
        //    {
        //        var wires = Wires.ToArray();
        //        var ret = wires.Where(o => !o.IsSeeder).Count();
        //        wires.DisposeAll();
        //        return ret;
        //    }
        //}
        public Torrent Torrent { get; }
        public File File { get; }
        public TorrentFileStateInfo(Torrent torrent, File file)
        {
            Torrent = torrent;
            File = file;
            Wires = torrent.Wires;
            Files = torrent.Files;
        }
    }
    public class TorrentWireStateInfo
    {
        public ProgressBarStyle ProgressBarStyle
        {
            get
            {
                return Wire.IsSeeder ? ProgressBarStyle.Success : (
                    !Uploading && !Downloading ? ProgressBarStyle.Warning : ProgressBarStyle.Info
                );
            }
        }
        public bool Uploading => UploadSpeed > 0d;
        public bool Downloading => DownloadSpeed > 0;
        public bool IsSeeder => Wire.IsSeeder;
        public double Downloaded => Wire.Downloaded;
        public double Uploaded => Wire.Uploaded;
        public double UploadSpeed => Wire.UploadSpeed();
        public double DownloadSpeed => Wire.DownloadSpeed();
        public string RemoteAddress => Wire.RemoteAddress ?? "";
        public double PeerProgress
        {
            get
            {
                float totalPieces = Torrent.Bitfield?.Length ?? 0;
                float donePieces = Wire.IsSeeder ? totalPieces : Wire.PeerPieces?.PopCount() ?? 0;
                var percentDone = totalPieces == 0 ? 0 : donePieces / totalPieces;
                return percentDone;
            }
        }
        public string Type => Wire.Type;
        public string PeerId => Wire.PeerId;
        //public int Seeds
        //{
        //    get
        //    {
        //        var wires = Wires.ToArray();
        //        var ret = wires.Where(o => o.IsSeeder).Count();
        //        wires.DisposeAll();
        //        return ret;
        //    }
        //}
        //public int Peers
        //{
        //    get
        //    {
        //        var wires = Wires.ToArray();
        //        var ret = wires.Where(o => !o.IsSeeder).Count();
        //        wires.DisposeAll();
        //        return ret;
        //    }
        //}
        public Torrent Torrent { get; }
        public Wire Wire { get; }
        public TorrentWireStateInfo(Torrent torrent, Wire file)
        {
            Torrent = torrent;
            Wire = file;
        }
    }
}
