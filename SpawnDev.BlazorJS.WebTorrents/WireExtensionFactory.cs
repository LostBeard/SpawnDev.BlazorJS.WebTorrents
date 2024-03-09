using System.Xml.Linq;

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

    public interface IWireExtensionFactory
    {
        FuncCallback<Wire, WireExtension> CreateWireExtension { get; }
        string WireExtensionName { get; }
    }

    // https://github.com/webtorrent/bittorrent-protocol#extension-api
    public abstract class WireExtensionFactory : IDisposable, IWireExtensionFactory
    {
        public FuncCallback<Wire, WireExtension> CreateWireExtension { get; protected set; }
        public string WireExtensionName { get; protected set; }
        public string LoadedPropName { get; protected set; }
        public Type ExtensionType { get; protected set; }
        public WireExtensionFactory(string extensionName, Type extensionType)
        {
            LoadedPropName = $"__{extensionName}__loaded";
            WireExtensionName = extensionName;
            ExtensionType = extensionType;
            CreateWireExtension = new FuncCallback<Wire, WireExtension>(CreateExtension);
        }

        /// <summary>
        /// The default CreateExtension method will set a property on the wire to indicate this extension was loaded. That property is checked here.
        /// </summary>
        /// <param name="wire"></param>
        /// <returns></returns>
        public bool IsExtensionLoaded(Wire wire) => wire != null && wire.JSRef.Get<bool>(LoadedPropName);

        public List<WireExtension> WireExtensions { get; private set; } = new List<WireExtension>();

        public delegate void WireExtensionCreatedDelegate(WireExtension wireExtension);
        public event WireExtensionCreatedDelegate ExtensionCreated;
        public void Use(Wire wire) => wire.Use(this);
        protected virtual WireExtension CreateExtension(Wire wire)
        {
            Console.WriteLine("CreateExtension");
            wire?.JSRef?.Set(LoadedPropName, true);
            var ret = (WireExtension)Activator.CreateInstance(ExtensionType, wire, WireExtensionName)!;
            WireExtensions.Add(ret);
            ExtensionCreated?.Invoke(ret);
            return ret;
        }
        public void Dispose()
        {
            CreateWireExtension.Dispose();
        }
    }
    public class WireExtensionFactory<T> : WireExtensionFactory where T : WireExtension
    {
        public WireExtensionFactory(string extensionName) : base(extensionName, typeof(T)) { }
    }
}
