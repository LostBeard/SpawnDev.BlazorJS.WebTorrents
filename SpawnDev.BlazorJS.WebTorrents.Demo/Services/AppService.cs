using SpawnDev.BlazorJS.WebTorrents.Demo.Shared;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Services
{
    public class AppService : IAsyncBackgroundService
    {
        //
        /// <summary>
        /// Completes when the service is ready
        /// </summary>
        public Task Ready => _Ready ??= InitAsync();
        private Task? _Ready = null;
        public string PosterHref { get; set; }
        public event Action OnStateChanged;
        public void StateHasChanged() => OnStateChanged?.Invoke();
        WebTorrentService WebTorrentService;
        MimeTypeService MimeTypeService;
        public AppService(WebTorrentService webTorrentService, MimeTypeService mimeTypeService)
        {
            MimeTypeService = mimeTypeService;
            WebTorrentService = webTorrentService;
        }
        async Task InitAsync()
        {
            await WebTorrentService.Ready;
            await WebTorrentService.Client!.RegisterServerServiceWorker();
        }
        // Content Viewer
        public bool ViewerFileSet => ViewerFile != null;
        public Torrent? ViewerTorrent { get; private set; }
        public File? ViewerFile { get; private set; }
        public string? ViewerContentType { get; private set; }
        public void UnsetContentViewerFile() => SetContentViewerFile(null, null);
        public void SetContentViewerFile(Torrent? torrent, File? file)
        {
            ViewerTorrent = torrent;
            ViewerFile = torrent == null ? null : file;
            ViewerContentType = file == null ? null : MimeTypeService.GetExtensionMimeType(file.Name);
            StateHasChanged();
        }
        // Trackers
        public string SelectedTrackersDataGridItemInstanceId => SelectedTrackersDataGridItem?.InstanceId ?? "";
        public TrackersDataGridItem? SelectedTrackersDataGridItem { get; set; }
        public async Task SelectTrackersDataGridItem(TrackersDataGridItem? trackersDataGridItem)
        {
            var instanceId = trackersDataGridItem?.InstanceId ?? "";
            if (instanceId != SelectedTorrentsDataGridItemInstanceId)
            {
                SelectedTrackersDataGridItem = trackersDataGridItem;
                StateHasChanged();
                await Task.Yield();
            }
        }
        // Torrents
        public string SelectedTorrentsDataGridItemInstanceId => SelectedTorrentsDataGridItem?.InstanceId ?? "";
        public TorrentsDataGridItem? SelectedTorrentsDataGridItem { get; set; }
        public async Task SelectTorrentsDataGridItem(TorrentsDataGridItem? torrentsDataGridItem)
        {
            var instanceId = torrentsDataGridItem?.InstanceId ?? "";
            if (instanceId != SelectedTorrentsDataGridItemInstanceId)
            {
                SelectedTorrentsDataGridItem = torrentsDataGridItem;
                PosterHref = "";
                StateHasChanged();
                await Task.Yield();
                if (torrentsDataGridItem != null)
                {
                    using var posterFile = await WebTorrentService.GetTorrentPosterFile(torrentsDataGridItem.Torrent);
                    // if still selected
                    if (SelectedTorrentsDataGridItemInstanceId == instanceId)
                    {
                        PosterHref = posterFile?.StreamURL ?? "";
                        StateHasChanged();
                    }
                }
            }
        }
        // Wires
        public string SelectedWiresDataGridItemInstanceId => SelectedWiresDataGridItem?.InstanceId ?? "";
        public WiresDataGridItem? SelectedWiresDataGridItem { get; set; }
        public async Task SelectWiresDataGridItem(WiresDataGridItem? wiresDataGridItem)
        {
            var instanceId = wiresDataGridItem?.InstanceId ?? "";
            if (instanceId != SelectedTorrentsDataGridItemInstanceId)
            {
                SelectedWiresDataGridItem = wiresDataGridItem;
                StateHasChanged();
                await Task.Yield();
            }
        }
        // Files
        public string SelectedFilesDataGridItemInstanceId => SelectedFilesDataGridItem?.InstanceId ?? "";
        public FilesDataGridItem? SelectedFilesDataGridItem { get; set; }
        public async Task SelectFilesDataGridItem(FilesDataGridItem? filesDataGridItem)
        {
            var instanceId = filesDataGridItem?.InstanceId ?? "";
            if (instanceId != SelectedTorrentsDataGridItemInstanceId)
            {
                SelectedFilesDataGridItem = filesDataGridItem;
                StateHasChanged();
                await Task.Yield();
            }
        }
    }
}
