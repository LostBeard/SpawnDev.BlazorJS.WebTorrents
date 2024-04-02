using Radzen;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Shared
{
    public class FileStateInfo
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
                Torrent.NumPeers == 0 ? ProgressBarStyle.Danger : (
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
        public Torrent Torrent { get; }
        public File File { get; }
        public FileStateInfo(Torrent torrent, File file)
        {
            Torrent = torrent;
            File = file;
            Wires = torrent.Wires;
            Files = torrent.Files;
        }
    }
}
