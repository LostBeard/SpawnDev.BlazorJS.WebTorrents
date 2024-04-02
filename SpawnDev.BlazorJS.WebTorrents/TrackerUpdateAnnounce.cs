using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class TrackerUpdateAnnounce : TrackerUpdateData
    {
        public TrackerUpdateAnnounce(IJSInProcessObjectReference _ref) : base(_ref) { }
        public string InfoHash => JSRef.Get<string>("infoHash");
        public string Announce => JSRef.Get<string>("announce");
        public int Interval=> JSRef.Get<int>("interval");
        public int Complete => JSRef.Get<int>("complete");
        public int Incomplete => JSRef.Get<int>("incomplete");
    }
}
