using Microsoft.AspNetCore.Components;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Shared
{
    public partial class ContentViewer
    {
        [Parameter]
        public Torrent? Torrent { get; set; }

        [Parameter]
        public File? File { get; set; }

        [Parameter]
        public string? ContentType { get; set; }

        private string? _TorrentId { get; set; }
        private string? _FilePath { get; set; }
        string? ContentTypeMajor { get; set; }

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
            ContentTypeMajor = ContentType == null ? null : ContentType.Split('/')[0];
        }
    }
}
