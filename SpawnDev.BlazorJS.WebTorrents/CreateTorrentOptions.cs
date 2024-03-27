using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://github.com/webtorrent/create-torrent#createtorrentinput-opts-function-callback-err-torrent-
    /// <summary>
    /// Options used when calling webTorrent client seed method
    /// https://github.com/webtorrent/create-torrent#createtorrentinput-opts-function-callback-err-torrent-
    /// </summary>
    public class CreateTorrentOptions
    {
        /// <summary>
        /// name of the torrent (default = basename of `path`, or 1st file's name)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; } = null;
        /// <summary>
        /// free-form textual comments of the author
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Comment { get; set; } = null;
        /// <summary>
        /// name and version of program used to create torrent
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CreatedBy { get; set; } = null;

        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public DateTime? creationDte { get; set; } = null;

        /// <summary>
        /// remove hidden and other junk files? (default = true)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? FilterJunkFiles { get; set; } = null;
        /// <summary>
        /// is this a private .torrent? (default = false)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Private { get; set; } = null;
        /// <summary>
        /// force a custom piece length (number of bytes)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PieceLength { get; set; } = null;
        /// <summary>
        /// custom trackers (array of arrays of strings) (see [bep12](http://www.bittorrent.org/beps/bep_0012.html))<br />
        /// If announceList is omitted, the following trackers will be included automatically:<br />
        /// udp://tracker.leechers-paradise.org:6969<br />
        /// udp://tracker.coppersurfer.tk:6969<br />
        /// udp://tracker.opentrackr.org:1337<br />
        /// udp://explodie.org:6969<br />
        /// udp://tracker.empire-js.us:1337<br />
        /// wss://tracker.btorrent.xyz<br />
        /// wss://tracker.openwebtorrent.com<br />
        /// wss://tracker.webtorrent.dev<br />
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<List<string>>? AnnounceList { get; set; } = null;
        /// <summary>
        /// web seed urls (see [bep19](http://www.bittorrent.org/beps/bep_0019.html))
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? UrlList { get; set; } = null;
        /// <summary>
        /// add non-standard info dict entries, e.g. info.source, a convention for cross-seeding
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Info { get; set; } = null;

        // onProgress
    }
}
