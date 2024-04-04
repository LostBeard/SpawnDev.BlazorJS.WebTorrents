using SpawnDev.BlazorJS.WebTorrents.Demo.Shared;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Services
{
    public class AppService
    {
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
                    var posterHref = await WebTorrentService.GetTorrentPoster(torrentsDataGridItem.Torrent);
                    // if still selected
                    if (SelectedTorrentsDataGridItemInstanceId == instanceId)
                    {
                        PosterHref = posterHref;
                        StateHasChanged();
                    }
                }
            }
        }
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
    }
}
