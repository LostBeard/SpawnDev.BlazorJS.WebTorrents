using Microsoft.Extensions.DependencyInjection;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.Toolbox;
using System.Text.RegularExpressions;
using System.Web;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md
    public class WebTorrentService : IAsyncBackgroundService, IDisposable
    {

        public static Dictionary<string, string> CCMagnets = new Dictionary<string, string>   {
            { "Big Buck Bunny", "magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c&dn=Big+Buck+Bunny&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fbig-buck-bunny.torrent"},
            { "Cosmos Laundromat", "magnet:?xt=urn:btih:c9e15763f722f23e98a29decdfae341b98d53056&dn=Cosmos+Laundromat&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fcosmos-laundromat.torrent" },
            { "Sintel", "magnet:?xt=urn:btih:08ada5a7a6183aae1e09d831df6748d566095a10&dn=Sintel&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fsintel.torrent" },
            { "Tears of Steel", "magnet:?xt=urn:btih:209c8226b299b308beaf2b9cd3fb49212dbd13ec&dn=Tears+of+Steel&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Ftears-of-steel.torrent" },
        };
        public bool Verbose { get; set; }
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
        public WebTorrent? Client { get; private set; } = null;
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
        internal WebTorrentOptions? WebTorrentOptions { get; set; }
        /// <summary>
        /// If true, recent torrents will be saved when metadata is ready and loaded on startup
        /// </summary>
        public bool EnableRecent { get; set; } = true;
        /// <summary>
        /// if true and LoadRecentOnStartup is true, recent torrents will be loaded on startup in a paused state
        /// </summary>
        public bool LoadRecentPaused { get; set; }
        /// <summary>
        /// If recent torrents should be loaded with the deselect flag set to true
        /// </summary>
        public bool LoadRecentDeselected { get; set; }
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
            WireExtensionServices = serviceDescriptors.Where(o => typeof(IExtensionFactory).IsAssignableFrom(o.ServiceType) || typeof(IExtensionFactory).IsAssignableFrom(o.ImplementationType)).ToList();
        }
        FileSystemDirectoryHandle? StorageDir = null;

        public static List<string> DefaultTrackers { get; } = new List<string>
        {
            "udp://tracker.leechers-paradise.org:6969",
            "udp://tracker.coppersurfer.tk:6969",
            "udp://tracker.opentrackr.org:1337",
            "udp://explodie.org:6969",
            "udp://tracker.empire-js.us:1337",
            "wss://tracker.btorrent.xyz",
            "wss://tracker.openwebtorrent.com",
            "wss://tracker.webtorrent.dev"
        };

        /// <summary>
        /// Trackers currently set in client.tracker options
        /// </summary>
        public string[] Announce => Client?.Tracker?.Announce ?? new string[0];

        /// <summary>
        /// Returns trackers to use for seeding based on client options and known public trackers.
        /// </summary>
        public string[] SeedTrackers => Announce.Length > 0 ? Announce : DefaultTrackers.ToArray();

        public async Task InitAsync()
        {
            if (BeenInit) return;
            BeenInit = true;
            if (IsDisposed) return;
            await WebTorrent.ImportWebTorrent();

            JS.Set("_WebTorrentOptions", WebTorrentOptions);

            Client = WebTorrentOptions == null ? new WebTorrent() : new WebTorrent(WebTorrentOptions);

            JS.Set("_Client", Client);

            Client.OnError += WebTorrent_OnError;
            Client.OnTorrent += WebTorrent_OnTorrent;
            Client.OnAdd += WebTorrent_OnAdd;
            Client.OnRemove += WebTorrent_OnRemove;
            // storage
            using var navigator = JS.Get<Navigator>("navigator");
            using var storage = navigator.Storage;
            StorageDir = await storage.GetDirectory();
#if DEBUG
            JS.Set("_wt", Client);
            JS.Set("_clearTorrentStorage", new ActionCallback<string?>(async (string? name) =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    await Client.ClearTorrentStorage();
                }
                else
                {
                    await Client.ClearTorrentStorage(name);
                }
            }));
            JS.Set("_getTorrentStorage", new AsyncFuncCallback<List<string>>(Client.GetTorrentStorageNames));
#endif
            if (EnableRecent)
            {
                await LoadRecent(LoadRecentPaused, LoadRecentDeselected);
            }
        }

        // magnet:?xt=urn:btih:08ada5a7a6183aae1e09d831df6748d566095a10'
        public static bool IsMagnet(string value)
        {
            if (value == null) return false;
            return value.StartsWith(@"magnet:?xt=urn:btih:");// Regex.IsMatch(value, @"^magnet:\?xt=urn:btih:[a-f0-9]{40}\S*$", RegexOptions.IgnoreCase);
        }
        public static bool IsInfoHash(string value)
        {
            if (value == null) return false;
            return Regex.IsMatch(value, "^[a-f0-9]{40}$", RegexOptions.IgnoreCase);
        }

        public static string InfoHashToMagnet(string value, IEnumerable<string>? addTrackers = null, bool addDefaultTrackers = true)
        {
            var ret = $"magnet:?xt=urn:btih:{value}&dn:UNKNOWN";
            var trackers = new List<string>();
            if (addDefaultTrackers) trackers.AddRange(DefaultTrackers);
            if (addTrackers != null) trackers.AddRange(addTrackers);
            var urlEncodedTrackers = trackers.Count == 0 ? "" : "&tr=" + string.Join("&tr=", trackers.Select(o => HttpUtility.UrlEncode(o)));
            if (!string.IsNullOrEmpty(urlEncodedTrackers)) ret += urlEncodedTrackers;
            return ret;
        }

        /// <summary>
        /// Returns true of the service worker is already enable or if it was started
        /// </summary>
        /// <returns></returns>
        public async Task<bool> EnableServer()
        {
            if (Client == null) return false;
            if (ServiceWorkerEnabled) return true;
            ServiceWorkerEnabled = await Client.CreateServer();
            return ServiceWorkerEnabled;
        }
        void Torrent_OnMetadata(Torrent torrent)
        {
            if (Verbose) JS.Log("Torrent_OnMetadata", torrent.InfoHash);
        }
        void Torrent_OnWire(Torrent torrent, Wire wire)
        {
            if (Verbose) JS.Log("Torrent_OnWireAdd", torrent.InfoHash, wire.PeerId);
            wire.Once("close", () =>
            {
                if (Verbose) JS.Log("Torrent_OnWireRemove", torrent.InfoHash, wire.PeerId);
                OnTorrentWireRemove?.Invoke(torrent, wire);
            });
            OnTorrentWireAdd?.Invoke(torrent, wire);
            var wireExtensionFactoryServices = WireExtensionServices.Select(o => (IExtensionFactory)ServiceProvider.GetRequiredService(o.ServiceType)).ToList();
            foreach (var factory in wireExtensionFactoryServices)
            {
                wire.Use(torrent, factory.ExtensionName, factory.CreateExtension);
            }
            //wire.OnBitfield += (bitfield) =>
            //{
            //    if (Verbose) JS.Log("Wire_OnBitfield", bitfield);
            //};
            //wire.OnRequest += (index, offset, length, d) => {
            //    JS.Log("Wire_OnRequest", index, offset, length, d);
            //};
            //wire.OnPiece += (index) => {
            //    JS.Log("Wire_OnPiece", index);
            //};
            //wire.OnHave += (a) => {
            //    JS.Log("Wire_OnHave", a);
            //};
        }
        void Torrent_OnNoPeers(Torrent torrent, string announceType)
        {
            //if (Verbose) JS.Log("Torrent_OnNoPeers", torrent.InfoHash, announceType);
        }
        void Torrent_OnReady(Torrent torrent)
        {
            if (Verbose) JS.Log("Torrent_OnReady", torrent);
            using var discovery = torrent.Discovery;
            discovery.OnPeer += Discovery_OnPeer;
            torrent.Once("close", () =>
            {
                using var discovery = torrent.Discovery;
                discovery.OnPeer += Discovery_OnPeer;
            });
            
        }
        void Torrent_OnError(Torrent torrent, JSObject? error)
        {
            if (Verbose) JS.Log("Torrent_OnError", torrent, error);
        }
        void WebTorrent_OnAdd(Torrent torrent)
        {
            if (Verbose) JS.Log("WebTorrent_OnAdd", torrent);
            var onReady = new Action(() => Torrent_OnReady(torrent));
            var onWire = new Action<Wire>((wire) => Torrent_OnWire(torrent, wire));
            var onNoPeers = new Action<string>((announceType) => Torrent_OnNoPeers(torrent, announceType));
            var onMetadata = new Action(() => Torrent_OnMetadata(torrent));
            var onError = new Action<JSObject?>((error) => Torrent_OnError(torrent, error));
            torrent.Once("close", () =>
            {
                if (Verbose) JS.Log("Torrent_OnClose", torrent);
                torrent.OnReady -= onReady;
                torrent.OnWire -= onWire;
                torrent.OnNoPeers -= onNoPeers;
                torrent.OnMetadata -= onMetadata;
                torrent.OnError -= onError;
                OnTorrentRemove?.Invoke(torrent);
            });
            torrent.OnReady += onReady;
            torrent.OnWire += onWire;
            torrent.OnNoPeers += onNoPeers;
            torrent.OnMetadata += onMetadata;
            torrent.OnError += onError;
            OnTorrentAdd?.Invoke(torrent);
        }
        void Discovery_OnPeer(DiscoveredPeer peer, string source)
        {
            if (Verbose) JS.Log("Discovery_OnPeer", peer, source);
        }
        void WebTorrent_OnRemove(Torrent torrent)
        {
            if (Verbose) JS.Log("WebTorrent_OnRemove", torrent);
            if (!string.IsNullOrEmpty(torrent.InfoHash))
            {
                _ = DeleteRecent(torrent.InfoHash);
            }
        }
        void WebTorrent_OnTorrent(Torrent torrent)
        {
            if (Verbose) JS.Log("WebTorrent_OnTorrent", torrent);
            if (EnableRecent)
            {
                _ = AddRecent(torrent);
            }
        }
        void WebTorrent_OnError(JSObject error)
        {
            if (Verbose) JS.Log("OnError", error);
        }
        async Task<List<Torrent>> LoadRecent(bool paused = false, bool deselect = false)
        {
            var ret = new List<Torrent>();
            if (Client == null) return ret;
            var recent = await GetRecentTorrents();
            foreach (var r in recent)
            {
                var torrentFilePath = $"recent/{r.InfoHash}/main.torrent";
                if (!await StorageDir!.FilePathExists(torrentFilePath))
                {
                    if (Verbose) JS.Log($"Skipping: {r.InfoHash}");
                    continue;
                }
                var torrentFile = await StorageDir!.ReadUint8Array($"recent/{r.InfoHash}/main.torrent");
                var torrent = Client.Add(torrentFile, new AddTorrentOptions { Paused = paused, Deselect = deselect });
                ret.Add(torrent);
            }
            return ret;
        }
        async Task<List<RecentTorrent>> GetRecentTorrents()
        {
            var ret = new List<RecentTorrent>();
            await StorageDir!.CreatePathDirectory("recent");
            var dirs = await StorageDir!.GetPathDirectoryHandles("recent");
            foreach (var dir in dirs)
            {
                try
                {
                    if (await dir.FilePathExists("torrent.json"))
                    {
                        var info = await dir.ReadJSON<RecentTorrent>("torrent.json");
                        if (info != null && !string.IsNullOrEmpty(info.MagnetURI))
                        {
                            if (Verbose) JS.Log($"Recent torrent found: {info.Name}");
                            ret.Add(info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    JS.Log($"Exception: {ex.Message}");
                }
            }
            return ret;
        }
        async Task DeleteRecent(string infoHash)
        {
            try
            {
                await StorageDir!.RemovePath($"recent/{infoHash}", true);
            }
            catch (Exception ex)
            {
                JS.Log($"DeleteRecent failed: {ex.Message}");
            }
        }
        async Task AddRecent(Torrent torrent)
        {
            try
            {
                await StorageDir!.Write($"recent/{torrent.InfoHash}/main.torrent", torrent.TorrentFile);
                await StorageDir!.WriteJSON($"recent/{torrent.InfoHash}/torrent.json", new RecentTorrent
                {
                    InfoHash = torrent.InfoHash,
                    Paused = torrent.Paused,
                    MagnetURI = torrent.MagnetURI,
                    Name = torrent.Name,
                });
            }
            catch (Exception ex)
            {
                JS.Log($"AddRecent failed: {ex.Message}");
            }
        }
        bool IsDisposed = false;
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Client?.Dispose();
        }
        /// <summary>
        /// Returns true if a torrent with the given torrentId exists
        /// </summary>
        /// <param name="torrentId"></param>
        /// <returns></returns>
        public async Task<bool> GetTorrentExists(string torrentId)
        {
            if (Client == null) return false;
            using var torrent = await Client.Get(torrentId);
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
            if (Client == null) throw new NullReferenceException(nameof(Client));
            var torrent = await Client.Get(torrentId);
            if (torrent != null) return torrent;
            if (torrent == null && !allowAdd) return null;
            if (Verbose) JS.Log("adding torrent");
            var options = new AddTorrentOptions();
            //options.Announce = AnnounceTrackers;
            return Client.Add(torrentId, options);
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
            if (Client == null) throw new NullReferenceException(nameof(Client));
            var torrent = await Client.Get(torrentId);
            if (torrent != null) return torrent;
            if (Verbose) JS.Log("adding torrent");
            //options.Announce = AnnounceTrackers;
            return Client.Add(torrentId, addTorrentOptions);
        }
        /// <summary>
        /// Removes all completed torrents and deletes all related data
        /// </summary>
        /// <param name="hasBeenConfirmed"></param>
        /// <returns></returns>
        public int RemoveCompleted(bool hasBeenConfirmed)
        {
            var ret = 0;
            using var torrents = Client!.Torrents;
            torrents.ToArray().UsingEach(t =>
            {
                if (t.Done)
                {
                    if (hasBeenConfirmed) t.Destroy(new DestroyTorrentOptions { DestroyStore = true });
                    ret++;
                }
            });
            return ret;
        }
        public int RemoveAllTorrents(bool hasBeenConfirmed, Func<Torrent, bool>? predicate = null)
        {
            var ret = 0;
            using var torrents = Client!.Torrents;
            torrents.ToArray().UsingEach(t => {
                if (predicate == null || predicate(t))
                {
                    if (hasBeenConfirmed) t.Destroy(new DestroyTorrentOptions { DestroyStore = true });
                    ret++;
                }
            });
            return ret;
        }

        string[] ImageExtensions { get; set; } = new string[] { "png", "jpg", "jpeg", "webm" };
        Dictionary<string, string> TorrentPosters = new Dictionary<string, string>();

        public async Task<string> GetTorrentPoster(Torrent torrent)
        {
            if (string.IsNullOrEmpty(torrent.InfoHash)) return "";
            if (TorrentPosters.TryGetValue(torrent.InfoHash, out var r)) return r;
            using var files = torrent.Files;
            foreach (File file in files.ToArray())
            {
                var ext = !file.Name.Contains(".") ? "" : file.Name.Substring(file.Name.LastIndexOf(".") + 1);
                var baseName = !file.Name.Contains(".") ? file.Name : file.Name.Substring(0, file.Name.LastIndexOf("."));
                if (baseName.Equals("poster", StringComparison.OrdinalIgnoreCase) && ImageExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                {
                    if (file.IsDone())
                    {
                        using var blob = await file.Blob();
                        var base64Url = await blob.ToDataURLAsync();
                        file.Dispose();
                        TorrentPosters[torrent.InfoHash] = base64Url!;
                        return base64Url!;
                    }
                }
                file.Dispose();
            }
            return "";
        }
    }
}
