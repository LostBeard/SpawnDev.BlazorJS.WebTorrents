namespace SpawnDev.BlazorJS.WebTorrents
{
    public static class IWireExtensionFactoryExtensions
    {
        /// <summary>
        /// Set the prototype.name property of the extension to IWireExtensionFactory.WireExtensionName
        /// </summary>
        /// <param name="_this"></param>
        public static void SetName(this IWireExtensionFactory _this)
        {
            using var fn = BlazorJSRuntime.JS.ReturnMe<JSObject>(_this.CreateWireExtension);
            // torrent.use() checks for Extension.prototype.name
            // it throws an exception if it does not exist
            // the below line sets it
            fn.JSRef!.Set("prototype.name", _this.RendezvousName);
        }
    }
}
