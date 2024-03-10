using Microsoft.Extensions.DependencyInjection;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.Toolbox;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md
    public class WebTorrentService : IDisposable, IAsyncBackgroundService
    {
        public WebTorrent? webtClient { get; private set; } = null;
        public List<string> AnnounceTrackers { get; private set; } = new List<string>();
        public string WebTorrentLibraryVersion { get; private set; } = "";
        BlazorJSRuntime JS;
        Function? WebTorrent { get; set; }
        public bool BeenInit { get; private set; }
        // Latest release
        // https://github.com/webtorrent/webtorrent/releases
        // current version is 2.0.15
        string latestVersionSrc = $"./_content/SpawnDev.BlazorJS.WebTorrents/webtorrent.min.js";
        public bool ServiceWorkerEnabled { get; private set; }
        ModuleNamespaceObject? WebTorrentModule { get; set; } = null;
        CallbackGroup _callbacks = new CallbackGroup();
        IServiceProvider _serviceProvider;
        IServiceCollection _serviceDescriptors;
        List<ServiceDescriptor> _wireExtensionServices;

        public bool Supported { get; private set; } 
        public Cache DefaultCache { get; private set; }
        public WebTorrentService(BlazorJSRuntime js, IServiceCollection serviceDescriptors, IServiceProvider serviceProvider)
        {
            JS = js;
            _serviceDescriptors = serviceDescriptors;
            _serviceProvider = serviceProvider;
            _wireExtensionServices = _serviceDescriptors.Where(o =>
                typeof(IWireExtensionFactory).IsAssignableFrom(o.ServiceType) || typeof(IWireExtensionFactory).IsAssignableFrom(o.ImplementationType)
            ).ToList();
        }

        public event Action<Torrent> OnTorrent;

        public event Action<Torrent, Wire> OnWire;

        // to delete FileSystem api data on chrome ....
        // chrome://settings/content/all?searchSubpage=localhost
        public async Task InitAsync()
        {
            if (BeenInit) return;
            BeenInit = true;
            if (IsDisposed) return;
            //
            using var caches = new CacheStorage();
            DefaultCache = await caches.Open("default");
            //
            WebTorrentModule = await JS.Import(latestVersionSrc);
            if (WebTorrentModule == null) throw new Exception("WebTorrentService could not be initialized.");
            WebTorrentLibraryVersion = WebTorrentModule.GetExport<string?>("default.VERSION") ?? "";
            WebTorrent = WebTorrentModule.GetExport<Function>("default");
            // set WebTorrent on the global scope so it can be used globally
            JS.Set("WebTorrent", WebTorrent);
            try
            {
                webtClient = new WebTorrent();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to create WebTorrent instance");
            }
            if (webtClient == null || webtClient.JSRef == null)
            {
                return;
            }
            Supported = true;
#if DEBUG
            JS.Set("_webtorrent", webtClient);
#endif
            webtClient.OnError += _OnError;
            webtClient.OnTorrent += _OnTorrent;
        }

        void OnTorrentWire(Torrent torrent, Wire wire)
        {
            JS.Log("OnWire", torrent.InfoHash, wire.PeerId);
            var wireExtensionFactoryServices = _wireExtensionServices.Select(o => (IWireExtensionFactory)_serviceProvider.GetRequiredService(o.ServiceType)).ToList();
            foreach (var factory in wireExtensionFactoryServices)
            {
                wire.Use(factory);
            }
            OnWire?.Invoke(torrent, wire);
        }

        void OnTorrentMetadata(Torrent torrent)
        {
            JS.Log("OnTorrentMetadata", torrent.InfoHash);
        }

        void _OnTorrent(Torrent torrent)
        {
            _ = Task.Run(async () =>
            {
                using var torrentFile = torrent.TorrentFile;
                var torrentFilename = $"/torrents/{torrent.InfoHash}.torrent";
                using var buffer = torrentFile.Buffer;
                await DefaultCache.WriteArrayBuffer(torrentFilename, buffer);
            });

            JS.Log("OnTorrent", torrent.InfoHash, torrent.Files.Count().ToString(), torrent.Name);
            TorrentSeen(torrent);
        }

        void _OnError(JSObject error)
        {
            JS.Log("OnError", error);
        }

        void _OnNoPeersError(Torrent torrent, string announceType)
        {
            JS.Log("NoPeers", torrent.InfoHash, announceType);
        }

        public Dictionary<string, Torrent> Torrents { get; } = new Dictionary<string, Torrent>();

        void TorrentSeen(Torrent torrent)
        {
            var infohash = torrent.InfoHash;
            if (string.IsNullOrEmpty(infohash)) return;
            if (!Torrents.ContainsKey(infohash))
            {
                torrent = JS.ReturnMe(torrent);
                Torrents.Add(infohash, torrent);
                var comment = torrent.Comment ?? "[NULL_COMMENT]";
                Console.WriteLine($"TorrentSeen: {infohash} Comment: {comment}");
                torrent.OnMetadata += () => OnTorrentMetadata(torrent);
                torrent.OnWire += (wire) => OnTorrentWire(torrent, wire);
                torrent.OnNoPeers += (announceType) => _OnNoPeersError(torrent, announceType);
                //if (string.IsNullOrEmpty(_appKeyTorrentMagnet) && _appKeyTorrent != null && _appKeyTorrent.InfoHash == infohash)
                //{
                //    _appKeyTorrentMagnet = _appKeyTorrent.MagnetURI;
                //    _appKeyTorrentInfohash = _appKeyTorrent.InfoHash;
                //    Console.WriteLine($"_appKeyTorrentInfohash: {_appKeyTorrentInfohash}");
                //}
                OnTorrent?.Invoke(torrent);
            }
        }

        public bool IsDisposed { get; private set; } = false;

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            webtClient?.Dispose();
            WebTorrent?.Dispose();
            WebTorrentModule?.Dispose();
            _callbacks.Dispose();
        }

        public async Task<bool> GetTorrentExists(string torrentId)
        {
            if (webtClient == null) return false;
            using var torrent = await webtClient.Get(torrentId);
            return torrent != null;
        }
        /// <summary>
        /// Returns a torrent with the given torrentId if it exists,<br />
        /// Else if allowAdd, it is created and added and then returned
        /// </summary>
        /// <param name="torrentId">usually the infoHash</param>
        /// <param name="allowAdd">If not found the torrent will be added</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<Torrent?> GetTorrent(string torrentId, bool allowAdd = true)
        {
            if (webtClient == null) throw new NullReferenceException(nameof(webtClient));
            var torrent = await webtClient.Get(torrentId);
            if (torrent != null) return torrent;
            if (torrent == null && !allowAdd) return null;
            Console.WriteLine("adding torrent");
            AddTorrentOptions options = new AddTorrentOptions();
            options.announce = AnnounceTrackers;
            return webtClient.Add(torrentId);
        }
    }
}
