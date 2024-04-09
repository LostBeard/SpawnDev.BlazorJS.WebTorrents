using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class HTTPTracker : Tracker
    {
        /// <summary>
        /// Returns true if the tracker looks like this type
        /// </summary>
        /// <param name="tracker"></param>
        /// <returns></returns>
        public static bool IsThisTackerType(Tracker tracker) => tracker.AnnounceUrl.StartsWith("http://") || tracker.AnnounceUrl.StartsWith("https://");
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public HTTPTracker(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// 
        /// </summary>
        public string ScrapeUrl => JSRef.Get<string>("scrapeUrl");
    }
}
