using Microsoft.Extensions.DependencyInjection;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md
    public class WebTorrentService : IAsyncBackgroundService, IDisposable
    {
        /// <summary>
        /// Called when a torrent is removed
        /// </summary>
        public event Action<Torrent> OnTorrentRemove;
        /// <summary>
        /// Called when a torrent is added
        /// </summary>
        public event Action<Torrent> OnTorrentAdd;
        /// <summary>
        /// Called when a torrent wire is added
        /// </summary>
        public event Action<Torrent, Wire> OnTorrentWireAdd;
        /// <summary>
        /// Called when a torrent wire is added
        /// </summary>
        public event Action<Torrent, Wire> OnTorrentWireRemove;
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
        
        private IServiceProvider ServiceProvider;
        private List<ServiceDescriptor> WireExtensionServices;
        // to delete FileSystem api data on chrome ....
        // chrome://settings/content/all?searchSubpage=localhost
        public WebTorrentService(BlazorJSRuntime js, IServiceCollection serviceDescriptors, IServiceProvider serviceProvider)
        {
            JS = js;
            ServiceProvider = serviceProvider;
            WireExtensionServices = serviceDescriptors.Where(o => typeof(IWireExtensionFactory).IsAssignableFrom(o.ServiceType) || typeof(IWireExtensionFactory).IsAssignableFrom(o.ImplementationType)).ToList();
        }
       
        public async Task InitAsync()
        {
            if (BeenInit) return;
            BeenInit = true;
            if (IsDisposed) return;
            await WebTorrent.ImportWebTorrent();
            WebTorrent = new WebTorrent();
            WebTorrent.OnError += WebTorrent_OnError;
            WebTorrent.OnTorrent += WebTorrent_OnTorrent;
            WebTorrent.OnAdd += WebTorrent_OnAdd;
            WebTorrent.OnRemove += WebTorrent_OnRemove;
#if DEBUG
            JS.Set("_wt", WebTorrent);
            JS.Set("_clearTorrentStorage", new ActionCallback<string?>(async (string? name) =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    await WebTorrent.ClearTorrentStorage();
                }
                else
                {
                    await WebTorrent.ClearTorrentStorage(name);
                }
            }));
#endif
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
        void Torrent_OnMetadata(Torrent torrent)
        {
            JS.Log("Torrent_OnMetadata", torrent.InfoHash);
        }
        void Torrent_OnWire(Torrent torrent, Wire wire)
        {
            JS.Log("Torrent_OnWireAdd", torrent.InfoHash, wire.PeerId);
            wire.Once("close", () =>
            {
                JS.Log("Torrent_OnWireRemove", torrent.InfoHash, wire.PeerId);
                OnTorrentWireRemove?.Invoke(torrent, wire);
            });
            OnTorrentWireAdd?.Invoke(torrent, wire);
            var wireExtensionFactoryServices = WireExtensionServices.Select(o => (IWireExtensionFactory)ServiceProvider.GetRequiredService(o.ServiceType)).ToList();
            foreach (var factory in wireExtensionFactoryServices)
            {
                wire.Use(factory);
            }
        }
        void Torrent_OnNoPeers(Torrent torrent, string announceType)
        {
            JS.Log("Torrent_OnNoPeersError", torrent.InfoHash, announceType);
        }
        void Torrent_OnError(Torrent torrent, JSObject? error)
        {
            JS.Log("Torrent_OnError", torrent, error);
        }
        void WebTorrent_OnAdd(Torrent torrent)
        {
            JS.Log("WebTorrent_OnAdd", torrent);
            var onWire = new Action<Wire>((wire) => Torrent_OnWire(torrent, wire));
            var onNoPeers = new Action<string>((announceType) => Torrent_OnNoPeers(torrent, announceType));
            var onMetadata = new Action(() => Torrent_OnMetadata(torrent));
            var onError = new Action<JSObject?>((error) => Torrent_OnError(torrent, error));
            torrent.Once("close", () => {
                JS.Log("Torrent_OnClose", torrent);
                torrent.OnWire -= onWire;
                torrent.OnNoPeers -= onNoPeers;
                torrent.OnMetadata -= onMetadata;
                torrent.OnError -= onError;
                OnTorrentRemove?.Invoke(torrent);
            });
            torrent.OnWire += onWire;
            torrent.OnNoPeers += onNoPeers;
            torrent.OnMetadata += onMetadata;
            torrent.OnError += onError;
            OnTorrentAdd?.Invoke(torrent);
        }
        void WebTorrent_OnRemove(Torrent torrent)
        {
            JS.Log("WebTorrent_OnRemove", torrent);
        }
        void WebTorrent_OnTorrent(Torrent torrent)
        {
            JS.Log("WebTorrent_OnTorrent", torrent);
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
