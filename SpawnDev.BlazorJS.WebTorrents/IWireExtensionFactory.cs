namespace SpawnDev.BlazorJS.WebTorrents
{
    public interface IWireExtensionFactory
    {
        FuncCallback<Wire, WireExtension> CreateWireExtension { get; }
        string WireExtensionName { get; }
    }
}
