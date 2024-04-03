namespace SpawnDev.BlazorJS.WebTorrents
{
    public static class TorrentExtensions
    {
        public static bool IsDone(this File _this) => _this.Progress >= 1d;
    }
}
