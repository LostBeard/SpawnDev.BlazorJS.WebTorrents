using Radzen;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Shared
{
    public class TrackersDataGridItem
    {
        public ProgressBarStyle ProgressBarStyle
        {
            get
            {
                return ProgressBarStyle.Success;
                //return Tracker.IsSeeder ? ProgressBarStyle.Success : (!Uploading && !Downloading ? ProgressBarStyle.Warning : ProgressBarStyle.Info);
            }
        }
        public int PeerCount => WebSocketTracker?.Peers?.Using(p => p.Length) ?? 0;
        public string InstanceId => Tracker.InstanceId;
        public string Type => Tracker.Type;
        public string AnnounceUrl => Tracker.AnnounceUrl;
        public Torrent Torrent { get; }
        public WebSocketTracker? WebSocketTracker { get; }
        public HTTPTracker? HTTPTracker { get; }
        public Tracker Tracker { get; }
        public TrackersDataGridItem(Torrent torrent, Tracker tracker)
        {
            Torrent = torrent;
            Tracker = tracker;
            WebSocketTracker = WebSocketTracker.IsThisTackerType(Tracker) ? Tracker.JSRefCopy<WebSocketTracker>() : null;
            HTTPTracker = HTTPTracker.IsThisTackerType(Tracker) ? Tracker.JSRefCopy<HTTPTracker>() : null;
        }
    }
}
