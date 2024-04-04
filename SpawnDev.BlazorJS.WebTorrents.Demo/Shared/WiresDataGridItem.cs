using BencodeNET.Torrents;
using Radzen;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Shared
{
    public class WiresDataGridItem
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
        public string RemoteAddress { get; set; }
        public double PeerProgress
        {
            get
            {
                float totalPieces = Torrent.Bitfield?.Length ?? 0;
                using var bitfield = Wire.IsSeeder ? null : Wire.PeerPieces;
                float donePieces = Wire.IsSeeder ? totalPieces : bitfield?.PopCount() ?? 0;
                var percentDone = totalPieces == 0 ? 0 : donePieces / totalPieces;
                return percentDone;
            }
        }
        public string InstanceId => Wire.InstanceId;
        public string Type => Wire.Type;
        public string PeerId => Wire.PeerId;
        public Torrent Torrent { get; }
        public Wire Wire { get; }
        public WiresDataGridItem(Torrent torrent, Wire wire)
        {
            Torrent = torrent;
            Wire = wire;
            if (Type == "webSeed")
            {
                //Object.values(_torrent._peers).filter(o => o.wire.peerId == _wire.peerId)
                var url = Torrent.Peers.UsingFirstOrDefault(peer => peer.Wire?.Using(w => w.InstanceId == InstanceId) ?? false)?.Using(peer => peer.Id) ?? "";
                if (!string.IsNullOrEmpty(url))
                {
                    try
                    {
                        RemoteAddress = new Uri(url).Host;
                    }
                    catch
                    {
                        RemoteAddress = url;
                    }
                }
            }
            else
            {
                RemoteAddress = Wire.RemoteAddress ?? "";
            }
        }
    }
}
