using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Options for client.add<br />
    /// https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#clientaddtorrentid-opts-function-ontorrent-torrent-
    /// </summary>
    public class AddTorrentOptions
    {
        /// <summary>
        /// Torrent trackers to use (added to list in .torrent or magnet uri)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Union<string, List<string>>? Announce { get; set; }
        /// <summary>
        /// Custom callback to allow sending extra parameters to the tracker
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Callback? GetAnnounceOpts { get; set; }
        /// <summary>
        /// Array of web seeds
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? UrlList { get; set; }
        /// <summary>
        /// Max number of simultaneous connections per web seed [default=4]
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxWebConns { get; set; }
        /// <summary>
        /// Folder to download files to (default=`/tmp/webtorrent/`)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Path { get; set; }
        /// <summary>
        /// If true, client will not share the hash with the DHT nor with PEX (default is the privacy of the parsed torrent)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Private { get; set; }
        /// <summary>
        /// Custom chunk store (must follow [abstract-chunk-store](https://www.npmjs.com/package/abstract-chunk-store) API)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JSObject? Store { get; set; }
        /// <summary>
        /// If truthy, client will delete the torrent's chunk store (e.g. files on disk) when the torrent is destroyed
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? DestroyStoreOnDestroy { get; set; }
        /// <summary>
        /// Number of chunk store entries (torrent pieces) to cache in memory [default=20]; 0 to disable caching
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? StoreCacheSlots { get; set; }
        /// <summary>
        /// Custom options passed to the store
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? StoreOpts { get; set; }
        /// <summary>
        /// If true, client will skip verification of pieces for existing store and assume it's correct
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? SkipVerify { get; set; }
        /// <summary>
        /// Piece selection strategy, `rarest` or `sequential`(defaut=`sequential`)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Strategy { get; set; }
        /// <summary>
        /// The amount of time (in seconds) to wait between each check of the `noPeers` event (default=30)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? NoPeersIntervalTime { get; set; }
        /// <summary>
        /// If true, create the torrent in a paused state (default=false)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Paused { get; set; }
        /// <summary>
        /// If true, create the torrent with no pieces selected (default=false)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Deselect { get; set; }
    }
}
