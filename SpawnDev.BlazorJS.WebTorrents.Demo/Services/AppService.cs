using SpawnDev.BlazorJS.WebTorrents.Demo.Shared;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Services
{
    public class AppService
    {
        string SelectedTorrentsDataGridItemInstanceId = "";
        public TorrentsDataGridItem? SelectedTorrentsDataGridItem { get; set; }
        public async Task SelectTorrentsDataGridItem(TorrentsDataGridItem? torrentsDataGridItem)
        {
            var instanceId = torrentsDataGridItem == null ? "" : torrentsDataGridItem.InstanceId;
            if (instanceId != SelectedTorrentsDataGridItemInstanceId)
            {
                SelectedTorrentsDataGridItemInstanceId = instanceId;
                SelectedTorrentsDataGridItem = torrentsDataGridItem;
                PosterHref = "";
                StateHasChanged();
                var posterHref = torrentsDataGridItem == null ? "" : await WebTorrentService.GetTorrentPoster(torrentsDataGridItem.Torrent);
                if (SelectedTorrentsDataGridItemInstanceId == instanceId)
                {
                    PosterHref = posterHref;
                    StateHasChanged();
                }
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
