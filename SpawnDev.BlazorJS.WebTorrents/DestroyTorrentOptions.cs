using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Options used when calling WebTorrent.Remove and Torrent.Destroy methods<br />
    /// https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#torrentdestroyopts-callback
    /// </summary>
    public class DestroyTorrentOptions
    {
        /// <summary>
        /// If true the torrent's data store will be destroyed. (default: false)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? DestroyStore { get; set; }
    }
}
