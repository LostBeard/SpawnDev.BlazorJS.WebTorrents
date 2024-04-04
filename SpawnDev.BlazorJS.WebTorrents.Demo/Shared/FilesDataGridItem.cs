using Radzen;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Shared
{
    public class FilesDataGridItem
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
        public string MimeType { get; }
        public string Path => File.Path;
        public string InstanceId => File.InstanceId;
        public string ImageHref { get; }
        public string Name => File.Name;
        public JSObjects.Array<Wire> Wires { get; }
        public JSObjects.Array<File> Files { get; }
        public Torrent Torrent { get; }
        public File File { get; }
        public FilesDataGridItem(Torrent torrent, File file, string? mimeType, string imageHref)
        {
            Torrent = torrent;
            File = file;
            Wires = torrent.Wires;
            Files = torrent.Files;
            MimeType = mimeType ?? "";
            ImageHref = imageHref;
        }
    }
}
