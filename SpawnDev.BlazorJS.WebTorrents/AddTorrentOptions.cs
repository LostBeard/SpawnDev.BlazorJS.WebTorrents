using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Options for WebTorrent.Add
    /// https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#clientaddtorrentid-opts-function-ontorrent-torrent-
    /// </summary>
    public class AddTorrentOptions
    {
        /// <summary>
        /// Torrent trackers to use (added to list in .torrent or magnet uri)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Announce { get; set; } = null;
        /// <summary>
        /// Custom callback to allow sending extra parameters to the tracker
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Callback? GetAnnounceOpts { get; set; } = null;
        /// <summary>
        /// Max number of simultaneous connections per web seed [default=4]
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxWebConns { get; set; } = null;
        /// <summary>
        /// If true, client will not share the hash with the DHT nor with PEX (default is the privacy of the parsed torrent)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Private { get; set; } = null;
        /// <summary>
        /// If truthy, client will delete the torrent's chunk store (e.g. files on disk) when the torrent is destroyed
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? DestroyStoreOnDestroy { get; set; } = null;
        /// <summary>
        /// If true, client will skip verification of pieces for existing store and assume it's correct
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? SkipVerify { get; set; } = null;
        /// <summary>
        /// Piece selection strategy, `rarest` or `sequential`(defaut=`sequential`)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Strategy { get; set; } = null;
        /// <summary>
        /// If true, create the torrent in a paused state (default=false)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Paused { get; set; } = null;
        /// <summary>
        /// If true, create the torrent with no pieces selected (default=false)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Deselect { get; set; } = null;
    }
}
