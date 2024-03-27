using Microsoft.Extensions.DependencyInjection;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md
    public class WebTorrentService : IAsyncBackgroundService, IDisposable
    {
        public event Action<Torrent> OnTorrent;
        public event Action<Torrent, Wire> OnWire;
        /// <summary>
        /// Returns true if the service worker server has been started
        /// </summary>
        public bool ServiceWorkerEnabled { get; private set; }
        /// <summary>
        /// WebTorrent instance
        /// </summary>
        public WebTorrent? WebTorrent { get; private set; } = null;
        /// <summary>
        /// Returns the library version reported by the library. This value does not always represent the actual release version.
        /// </summary>
        public string WebTorrentLibraryVersion { get; private set; } = "";
        /// <summary>
        /// The version of the bundled WebTorrent library release
        /// </summary>
        public string WebTorrentLibraryVersionActual { get; } = "2.2.0";
        private BlazorJSRuntime JS;
        private bool BeenInit = false;
        // Latest release
        // https://github.com/webtorrent/webtorrent/releases
        // current version is 2.2.0 (2024-03-26) (it reports itself as 2.1.36)
        private static string latestVersionSrc = $"./_content/SpawnDev.BlazorJS.WebTorrents/webtorrent.min.js";
        private IServiceProvider ServiceProvider;
        private List<ServiceDescriptor> WireExtensionServices;
        // to delete FileSystem api data on chrome ....
        // chrome://settings/content/all?searchSubpage=localhost
        public WebTorrentService(BlazorJSRuntime js, IServiceCollection serviceDescriptors, IServiceProvider serviceProvider)
        {
            JS = js;
            ServiceProvider = serviceProvider;
            WireExtensionServices = serviceDescriptors.Where(o => typeof(IWireExtensionFactory).IsAssignableFrom(o.ServiceType) || typeof(IWireExtensionFactory).IsAssignableFrom(o.ImplementationType)).ToList();
#if DEBUG
            JS.Set("_clearTorrentStorage", new ActionCallback<string?>(async (string? name) =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    await ClearTorrentStorage();
                }
                else
                {
                    await ClearTorrentStorage(name);
                }
            }));
#endif
        }
        public async Task InitAsync()
        {
            if (BeenInit) return;
            BeenInit = true;
            if (IsDisposed) return;
            var WebTorrentModule = await JS.Import(latestVersionSrc);
            if (WebTorrentModule == null) throw new Exception("WebTorrentService could not be initialized.");
            WebTorrentLibraryVersion = WebTorrentModule.GetExport<string?>("default.VERSION") ?? "";
            var WebTorrentClass = WebTorrentModule.GetExport<Function>("default");
            // set WebTorrent on the global scope so it can be used globally
            JS.Set("WebTorrent", WebTorrentClass);
            WebTorrent = new WebTorrent();
            WebTorrent.OnError += WebTorrent_OnError;
            WebTorrent.OnTorrent += WebTorrent_OnTorrent;
        }
        /// <summary>
        /// Returns true of the service worker is already enable or if it was started
        /// </summary>
        /// <returns></returns>
        public async Task<bool> EnableServer()
        {
            if (WebTorrent == null) return false;
            if (ServiceWorkerEnabled) return true;
            ServiceWorkerEnabled = await WebTorrent.CreateServer();
            return ServiceWorkerEnabled;
        }
        /// <summary>
        /// Remove all Torrent data from default Torrent store
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> ClearTorrentStorage()
        {
            var ret = new List<string>();
            using var navigator = JS.Get<Navigator>("navigator");
            using var storageManager = navigator.Storage;
            using var rootDir = await storageManager.GetDirectory();
            var entries = await rootDir.Values();
            foreach (var entry in entries!)
            {
                var pos = entry.Name.LastIndexOf(" - ");
                if (pos > -1 && entry.Kind == "directory")
                {
                    var entryTorrentName = entry.Name.Substring(0, pos);
                    Console.WriteLine($"Deleting cache for: {entryTorrentName}");
                    await rootDir.RemoveEntry(entry.Name, true);
                    ret.Add(entryTorrentName);
                }
            }
            Console.WriteLine($"ClearTorrentStorage done");
            return ret;
        }
        /// <summary>
        /// Remove Torrent data from default Torrent store
        /// </summary>
        /// <param name="torrentName"></param>
        /// <returns></returns>
        public async Task<bool> ClearTorrentStorage(string torrentName)
        {
            using var navigator = JS.Get<Navigator>("navigator");
            using var storageManager = navigator.Storage;
            using var rootDir = await storageManager.GetDirectory();
            var entries = await rootDir.Values();
            foreach (var entry in entries!)
            {
                var pos = entry.Name.LastIndexOf(" - ");
                if (pos > -1 && entry.Kind == "directory")
                {
                    var entryTorrentName = entry.Name.Substring(0, pos);
                    if (torrentName != entryTorrentName) continue;
                    Console.WriteLine($"Deleting cache for: {entryTorrentName}");
                    await rootDir.RemoveEntry(entry.Name, true);
                    return true;
                }
            }
            return false;
        }
        public Dictionary<string, Torrent> Torrents { get; } = new Dictionary<string, Torrent>();
        string WebTorrentSeenFlag => GetType().Name;
        public void TorrentSeen(Torrent torrent)
        {
            var torrentUID = torrent.JSRef!.Get<string?>(WebTorrentSeenFlag);
            if (!string.IsNullOrEmpty(torrentUID))
            {
                // already seen this
                return;
            }
            JS.Log("new TorrentSeen", torrent);
            torrentUID = Guid.NewGuid().ToString();
            torrent.JSRef!.Set(WebTorrentSeenFlag, torrentUID);
            torrent.OnMetadata += () => Torrent_OnMetadata(torrent);
            torrent.OnWire += (wire) => Torrent_OnWire(torrent, wire);
            torrent.OnNoPeers += (announceType) => Torrent_OnNoPeersError(torrent, announceType);
            torrent.OnError += (error) => Torrent_OnError(torrent, error);
            Torrents.Add(torrentUID, torrent);
        }
        void Torrent_OnMetadata(Torrent torrent)
        {
            JS.Log("OnTorrentMetadata", torrent.InfoHash);
        }
        void Torrent_OnWire(Torrent torrent, Wire wire)
        {
            JS.Log("OnWire", torrent.InfoHash, wire.PeerId);
            var wireExtensionFactoryServices = WireExtensionServices.Select(o => (IWireExtensionFactory)ServiceProvider.GetRequiredService(o.ServiceType)).ToList();
            foreach (var factory in wireExtensionFactoryServices)
            {
                wire.Use(factory);
            }
            OnWire?.Invoke(torrent, wire);
        }
        void Torrent_OnNoPeersError(Torrent torrent, string announceType)
        {
            JS.Log("NoPeers", torrent.InfoHash, announceType);
        }
        void Torrent_OnError(Torrent torrent, JSObject? error)
        {
            JS.Log("OnTorrentError", torrent, error);
        }
        void WebTorrent_OnTorrent(Torrent torrent)
        {
            TorrentSeen(torrent);
            OnTorrent?.Invoke(torrent);
            JS.Log("OnTorrent?.Invoke(torrent);", torrent);
        }
        void WebTorrent_OnError(JSObject error)
        {
            JS.Log("OnError", error);
        }
        bool IsDisposed = false;
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            WebTorrent?.Dispose();
        }
        /// <summary>
        /// Returns true if a torrent with the given torrentId exists
        /// </summary>
        /// <param name="torrentId"></param>
        /// <returns></returns>
        public async Task<bool> GetTorrentExists(string torrentId)
        {
            if (WebTorrent == null) return false;
            using var torrent = await WebTorrent.Get(torrentId);
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
            if (WebTorrent == null) throw new NullReferenceException(nameof(WebTorrent));
            var torrent = await WebTorrent.Get(torrentId);
            if (torrent != null) return torrent;
            if (torrent == null && !allowAdd) return null;
            Console.WriteLine("adding torrent");
            var options = new AddTorrentOptions();
            //options.Announce = AnnounceTrackers;
            return WebTorrent.Add(torrentId, options);
        }
        /// <summary>
        /// Returns a torrent with the given torrentId if it exists,<br />
        /// Else if allowAdd, it is created and added and then returned
        /// </summary>
        /// <param name="torrentId"></param>
        /// <param name="addTorrentOptions"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<Torrent?> GetTorrent(string torrentId, AddTorrentOptions addTorrentOptions)
        {
            if (WebTorrent == null) throw new NullReferenceException(nameof(WebTorrent));
            var torrent = await WebTorrent.Get(torrentId);
            if (torrent != null) return torrent;
            Console.WriteLine("adding torrent");
            //options.Announce = AnnounceTrackers;
            return WebTorrent.Add(torrentId, addTorrentOptions);
        }
    }
}
