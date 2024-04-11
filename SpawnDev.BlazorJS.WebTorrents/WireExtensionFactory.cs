//using System.Collections.ObjectModel;

//namespace SpawnDev.BlazorJS.WebTorrents
//{
//    public class WireExtensionFactory<T> : WireExtensionFactory where T : Extension
//    {
//        public WireExtensionFactory(string extensionName) : base(extensionName, typeof(T))
//        {
//            ExtensionCreated += WireExtensionFactory_ExtensionCreated;
//        }

//        private void WireExtensionFactory_ExtensionCreated(Extension wireExtension)
//        {
//            _WireExtensions.Add((T)wireExtension);
//        }
//        private  List<T> _WireExtensions { get; } = new List<T>();
//        public List<T> WireExtensions => _WireExtensions;
//    }

//    // https://github.com/webtorrent/bittorrent-protocol#extension-api
//    public abstract class WireExtensionFactory : IDisposable, IExtensionFactory
//    {
//        public FuncCallback<Wire, Extension> CreateWireExtension { get; protected set; }
//        public string ExtensionName { get; protected set; }
//        /// <summary>
//        /// The WireExtension Type that will be created for the wire
//        /// </summary>
//        public Type ExtensionType { get; protected set; }
//        public WireExtensionFactory(string extensionName, Type extensionType)
//        {
//            ExtensionName = extensionName;
//            ExtensionType = extensionType;
//            CreateWireExtension = new FuncCallback<Wire, Extension>(CreateExtension);
//        }
//        public delegate void WireExtensionCreatedDelegate(Extension wireExtension);
//        /// <summary>
//        /// Called when a new a new WireExtension is created
//        /// </summary>
//        public event WireExtensionCreatedDelegate ExtensionCreated;
//        /// <summary>
//        /// Ase this extension factory on the given Wire
//        /// </summary>
//        /// <param name="wire"></param>
//        public void Use(Wire wire) => wire.Use(this);
//        /// <summary>
//        /// Called when creating a new wire extension
//        /// </summary>
//        /// <param name="wire"></param>
//        /// <returns></returns>
//        protected virtual Extension CreateExtension(Wire wire)
//        {
//            var ret = (Extension)Activator.CreateInstance(ExtensionType, wire, WireExtensionName)!;
//            ExtensionCreated?.Invoke(ret);
//            return ret;
//        }
//        /// <summary>
//        /// Release disposable resources
//        /// </summary>
//        public void Dispose()
//        {
//            CreateWireExtension.Dispose();
//        }
//    }
//}
