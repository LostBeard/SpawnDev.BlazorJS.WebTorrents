//namespace SpawnDev.BlazorJS.WebTorrents
//{
//    /// <summary>
//    /// Adds a methods to IWireExtensionFactory
//    /// </summary>
//    public static class IWireExtensionFactoryExtensions
//    {
//        /// <summary>
//        /// Set the prototype.name property of the extension to IWireExtensionFactory.WireExtensionName<br /> 
//        /// wire.use() checks for Extension.prototype.name<br />
//        /// it throws an exception if it does not exist<br />
//        /// </summary>
//        /// <param name="_this"></param>
//        public static void SetName(this IExtensionFactory _this)
//        {
//            using var fn = BlazorJSRuntime.JS.ReturnMe<JSObject>(_this.CreateWireExtension);
//            fn.JSRef!.Set("prototype.name", _this.WireExtensionName);
//        }
//    }
//}
