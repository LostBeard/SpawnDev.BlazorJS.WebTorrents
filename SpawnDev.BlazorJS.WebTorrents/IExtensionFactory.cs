namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// An interface wire extensions should implement for use with Wire.Use()
    /// </summary>
    public interface IExtensionFactory
    {
        /// <summary>
        /// This method will be called whe na new instance of the wire extension is requested for a new wire.
        /// </summary>
        Extension CreateExtension(Torrent torrent, Wire wire);
        /// <summary>
        /// The name of the extension. This is used to identify the wire extension between peers.
        /// </summary>
        string ExtensionName { get; }
    }
}
