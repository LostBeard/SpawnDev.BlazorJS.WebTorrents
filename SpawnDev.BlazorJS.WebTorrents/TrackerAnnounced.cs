namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Custom class related to custom added property torrent.announced<br />
    /// that keeps the latest announce message from each tracker to allow active running totals of each tracker's seeders and peers<br />
    /// Not supported in official WebTorrents release; only LostBeard version
    /// </summary>
    public class TrackerAnnounced : TrackerAnnounce
    {
        /// <summary>
        /// Returns true if the announce data has expired
        /// </summary>
        /// <param name="leeway">The number of seconds to allow after the expected new message time before considering the announce data expired</param>
        /// <returns></returns>
        public bool Expired(int leeway = 10) => Time != null && DateTime.Now > ((DateTime)Time + TimeSpan.FromSeconds(Interval + leeway));
        /// <summary>
        /// Time the announce was received
        /// </summary>
        public EpochDateTime? Time { get; set; }
    }
}
