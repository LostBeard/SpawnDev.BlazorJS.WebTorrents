using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Options for client.seed<br />
    /// Contains the following types of options:<br />
    /// options for create-torrent (to allow configuration of the.torrent file that is created)<br />
    /// options for client.add<br />
    /// https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#clientseedinput-opts-function-onseed-torrent-
    /// </summary>
    public class SeedTorrentOptions : AddTorrentOptions
    {
        #region CreateTorrentOptions
        /// <summary>
        /// name of the torrent (default = basename of `path`, or 1st file's name)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; }
        /// <summary>
        /// free-form textual comments of the author
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Comment { get; set; }
        /// <summary>
        /// name and version of program used to create torrent
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CreatedBy { get; set; }
        /// <summary>
        /// Creation time in UNIX epoch format (default = now)<br />
        /// TODO - test this type works for this property
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? CreationDate { get; set; }
        /// <summary>
        /// remove hidden and other junk files? (default = true)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? FilterJunkFiles { get; set; }
        /// <summary>
        /// is this a private .torrent? (default = false)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Private { get; set; }
        /// <summary>
        /// force a custom piece length (number of bytes)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PieceLength { get; set; }
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
        /// TODO - test this works as expected
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string[]>? AnnounceList { get; set; }
        /// <summary>
        /// web seed urls (see [bep19](http://www.bittorrent.org/beps/bep_0019.html))
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? UrlList { get; set; }
        /// <summary>
        /// add non-standard info dict entries, e.g. info.source, a convention for cross-seeding
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Info { get; set; }
        /// <summary>
        /// called with the number of bytes hashed and estimated total size after every piece
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Callback? OnProgress { get; set; }
        #endregion
    }
}
