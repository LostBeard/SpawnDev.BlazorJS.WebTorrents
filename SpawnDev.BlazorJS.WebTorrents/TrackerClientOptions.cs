using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Options that can be used for the tracker client<br />
    /// https://github.com/webtorrent/bittorrent-tracker?tab=readme-ov-file#client<br />
    /// https://github.com/webtorrent/bittorrent-tracker/blob/a4f956e3cbc2534fb92bb9a8841cccb5224130e1/client.js#L17
    /// </summary>
    public class TrackerClientOptions
    {
        /// <summary>
        /// User-Agent header for http requests
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? UserAgent { get; set; }
        /// <summary>
        /// Client peer id
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PeerId { get; set; }
        /// <summary>
        /// Announce 
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? Announce { get; set; }
    }
}
