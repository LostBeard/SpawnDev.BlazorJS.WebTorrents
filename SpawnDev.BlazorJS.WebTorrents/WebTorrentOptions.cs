using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Optional options used when creating a new WebTorrent<br />
    /// https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#client--new-webtorrentopts
    /// </summary>
    public class WebTorrentOptions
    {
        /// <summary>
        /// Max number of connections per torrent (default=55)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxConns { get; set; } = null;
        /// <summary>
        /// DHT protocol node ID (default=randomly generated)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? NodeId { get; set; } = null;
        /// <summary>
        /// Wire protocol peer ID (default=randomly generated)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PeerId { get; set; } = null;
        /// <summary>
        /// Enable trackers (default=true), or options object for Tracker
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Tracker { get; set; } = null;
        /// <summary>
        /// Enable DHT (default=true), or options object for DHT
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Dht { get; set; } = null;
        /// <summary>
        /// Enable BEP14 local service discovery (default=true)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Lsd { get; set; } = null;
        /// <summary>
        /// Enable BEP11 Peer Exchange (default=true)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? UtPex { get; set; } = null;
        /// <summary>
        /// Enable BEP19 web seeds (default=true)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? WebSeeds { get; set; } = null;
        /// <summary>
        /// Enable BEP29 uTorrent transport protocol (default=true)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Utp { get; set; } = null;
        /// <summary>
        /// List of IP's to block
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Union<IEnumerable<string>, string>? BlockList { get; set; } = null;
        /// <summary>
        /// Max download speed (bytes/sec) over all torrents (default=-1)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? DownloadLimit { get; set; } = null;
        /// <summary>
        /// Max upload speed (bytes/sec) over all torrents (default=-1)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? UploadLimit { get; set; } = null;
    }
}
