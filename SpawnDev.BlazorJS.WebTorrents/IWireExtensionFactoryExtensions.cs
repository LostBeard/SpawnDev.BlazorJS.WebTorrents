namespace SpawnDev.BlazorJS.WebTorrents
{
    public static class IWireExtensionFactoryExtensions
    {
        public static void SetName(this IWireExtensionFactory _this)
        {
            using var fn = BlazorJSRuntime.JS.ReturnMe<JSObject>(_this.CreateWireExtension);
            // torrent.use() checks for Extension.prototype.name
            // it throws an exception if it does not exist
            fn.JSRef!.Set("prototype.name", _this.WireExtensionName);
        }
    }
}
