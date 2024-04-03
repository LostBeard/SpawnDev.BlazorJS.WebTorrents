using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Tracker announce message
    /// </summary>
    public class TrackerAnnounce
    {
        /// <summary>
        /// Should always be "announce"
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// InfoHash the announce is for
        /// </summary>
        public string InfoHash { get; set; }
        /// <summary>
        /// The announce message source url
        /// </summary>
        public string Announce { get; set; }
        /// <summary>
        /// How often, in seconds, the message can be expected
        /// </summary>
        public int Interval { get; set; }
        /// <summary>
        /// The number of peers that have complete copies of the torrent files
        /// </summary>
        public int Complete { get; set; }
        /// <summary>
        /// The number of peers that have incomplete copies of the torrent files
        /// </summary>
        public int Incomplete { get; set; }
    }
}
