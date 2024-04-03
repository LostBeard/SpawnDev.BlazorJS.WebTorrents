namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// MinimalSelectionItem class
    /// </summary>
    public class MinimalSelectionItem
    {
        /// <summary>
        /// Start position of the selection (inclusive)
        /// </summary>
        public long From { get; set; }
        /// <summary>
        /// The end of the selection (inclusive)
        /// </summary>
        public int To { get; set; }
    }
}
