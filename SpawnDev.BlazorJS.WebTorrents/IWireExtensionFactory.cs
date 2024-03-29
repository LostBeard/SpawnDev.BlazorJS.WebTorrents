namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// An interface wire extensions should implement for use with Wire.Use()
    /// </summary>
    public interface IWireExtensionFactory
    {
        /// <summary>
        /// This method will be called whe na new instance of the wire extension is requested for a new wire.
        /// </summary>
        FuncCallback<Wire, WireExtension> CreateWireExtension { get; }
        /// <summary>
        /// The name of the extension. This is used to identify the wire extension between peers.
        /// </summary>
        string WireExtensionName { get; }
    }
}
