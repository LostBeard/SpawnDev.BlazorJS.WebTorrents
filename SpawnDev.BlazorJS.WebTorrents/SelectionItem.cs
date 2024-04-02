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
    /// <summary>
    /// Selection class
    /// </summary>
    public class SelectionItem : MinimalSelectionItem
    {
        /// <summary>
        /// Offset inside the selection (usually 0)
        /// </summary>
        public long Offset { get; set; }
        /// <summary>
        /// The priority of the selection.
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// IsStreamSelection boolean
        /// </summary>
        public bool? IsStreamSelection { get; set; }
    }
}
