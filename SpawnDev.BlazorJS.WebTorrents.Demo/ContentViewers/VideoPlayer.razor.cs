using Microsoft.AspNetCore.Components;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.ContentViewers
{
    public partial class VideoPlayer
    {
        [Parameter]
        public Torrent? Torrent { get; set; }

        [Parameter]
        public File? File { get; set; }

        [Parameter]
        public string? ContentType { get; set; }

        string? Source = null;

        private string? _TorrentId { get; set; }
        private string? _FilePath { get; set; }

        protected override void OnParametersSet()
        {
            if (_TorrentId != Torrent?.InstanceId)
            {
                // torrent changed

                _TorrentId = Torrent?.InstanceId;
                _FilePath = File?.Path;
            }
            else if (_FilePath != File?.Path)
            {
                // file changed

                _FilePath = File?.Path;
            }
            else
            {
                return;
            }
            Source = File == null ? null : File.StreamURL;
        }
    }
}
