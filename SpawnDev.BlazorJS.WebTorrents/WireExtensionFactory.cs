namespace SpawnDev.BlazorJS.WebTorrents
{
    public class WireExtensionFactory<T> : WireExtensionFactory where T : WireExtension
    {
        public WireExtensionFactory(string extensionName) : base(extensionName, typeof(T)) { }
    }

    // https://github.com/webtorrent/bittorrent-protocol#extension-api
    public class WireExtensionFactory : IDisposable, IWireExtensionFactory
    {
        public FuncCallback<Wire, WireExtension> CreateWireExtension { get; protected set; }
        public string WireExtensionName { get; protected set; }
        public Type ExtensionType { get; protected set; }
        public WireExtensionFactory(string extensionName, Type extensionType)
        {
            WireExtensionName = extensionName;
            ExtensionType = extensionType;
            CreateWireExtension = new FuncCallback<Wire, WireExtension>(CreateExtension);
        }

        public delegate void WireExtensionCreatedDelegate(WireExtension wireExtension);
        public event WireExtensionCreatedDelegate ExtensionCreated;
        public void Use(Wire wire) => wire.Use(this);
        protected virtual WireExtension CreateExtension(Wire wire)
        {;
            var ret = (WireExtension)Activator.CreateInstance(ExtensionType, wire, WireExtensionName)!;
            ExtensionCreated?.Invoke(ret);
            return ret;
        }
        public void Dispose()
        {
            CreateWireExtension.Dispose();
        }
    }
}
