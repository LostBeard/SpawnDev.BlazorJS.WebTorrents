using Radzen;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Shared
{
    public class WireStateInfo
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
        public Torrent Torrent { get; }
        public Wire Wire { get; }
        public WireStateInfo(Torrent torrent, Wire file)
        {
            Torrent = torrent;
            Wire = file;
        }
    }
}
