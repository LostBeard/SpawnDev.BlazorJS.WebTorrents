using Microsoft.Extensions.DependencyInjection;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.Toolbox;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md
    /// <summary>
    /// The WebTorrentService imports the WebTorrent library and creates an instance of a WebTorrent client.
    /// </summary>
    public class WebTorrentService : IAsyncBackgroundService, IDisposable
    {
        /// <summary>
        /// A short list of some public domain torrents
        /// </summary>
        public static Dictionary<string, string> CCMagnets { get; } = new Dictionary<string, string>   {
            { "Big Buck Bunny", "magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c&dn=Big+Buck+Bunny&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fbig-buck-bunny.torrent"},
            { "Cosmos Laundromat", "magnet:?xt=urn:btih:c9e15763f722f23e98a29decdfae341b98d53056&dn=Cosmos+Laundromat&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fcosmos-laundromat.torrent" },
            { "Sintel", "magnet:?xt=urn:btih:08ada5a7a6183aae1e09d831df6748d566095a10&dn=Sintel&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fsintel.torrent" },
            { "Tears of Steel", "magnet:?xt=urn:btih:209c8226b299b308beaf2b9cd3fb49212dbd13ec&dn=Tears+of+Steel&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Ftears-of-steel.torrent" },
        };
        /// <summary>
        /// Known public trackers
        /// </summary>
        public static List<string> PublicTrackers { get; } = new List<string>
        {
            "wss://tracker.btorrent.xyz",
            "wss://tracker.openwebtorrent.com",
            "wss://tracker.webtorrent.dev",
            "udp://tracker.leechers-paradise.org:6969",
            "udp://tracker.coppersurfer.tk:6969",
            "udp://tracker.opentrackr.org:1337",
            "udp://explodie.org:6969",
            "udp://tracker.empire-js.us:1337",
        };
        /// <summary>
        /// If set to true verbose logging will be enabled
        /// </summary>
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
        /// The version of the bundled WebTorrent library release<br/>
        /// This is updated manually when the included library is updated
        /// </summary>
        public string BundledLibraryVersion { get; } = "2.5.7";
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
        private IServiceProvider ServiceProvider;
        private List<ServiceDescriptor> WireExtensionServices;
        /// <summary>
        /// Completes when the service is ready
        /// </summary>
        public Task Ready => _Ready ??= InitAsync();
        private Task? _Ready = null;
        // FileSystem api data on chrome....
        // chrome://settings/content/all
        /// <summary>
        /// Dependency injection constructor
        /// </summary>
        /// <param name="js"></param>
        /// <param name="serviceDescriptors"></param>
        /// <param name="serviceProvider"></param>
        public WebTorrentService(BlazorJSRuntime js, IServiceCollection serviceDescriptors, IServiceProvider serviceProvider)
        {
            JS = js;
            ServiceProvider = serviceProvider;
            WireExtensionServices = serviceDescriptors.Where(o => typeof(IExtensionFactory).IsAssignableFrom(o.ServiceType) || typeof(IExtensionFactory).IsAssignableFrom(o.ImplementationType)).ToList();
        }
        FileSystemDirectoryHandle? StorageDir = null;
        /// <summary>
        /// Trackers currently set in client.tracker options
        /// </summary>
        public string[] Announce => Client?.Tracker?.Announce ?? new string[0];
        /// <summary>
        /// Returns trackers to use for seeding based on client options and known public trackers.
        /// </summary>
        public string[] SeedTrackers => Announce.Length > 0 ? Announce : PublicTrackers.ToArray();
        /// <summary>
        /// Initialize the service. (Called automatically by SpawnDev.BlazorJS at start up if registered
        /// </summary>
        /// <returns></returns>
        async Task InitAsync()
        {
            if (BeenInit) return;
            BeenInit = true;
            if (IsDisposed) return;
            if (!JS.IsScope(GlobalScope.Window))
            {
                // WebTorrentService only supports the Window scope because WebRTC (which WebTorrent requires) is not supported in workers
                return;
            }
            await WebTorrent.ImportWebTorrent();
            Client = WebTorrentOptions == null ? new WebTorrent() : new WebTorrent(WebTorrentOptions);
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
            JS.Set("_clearTorrentStorage", Callback.Create(async (string? name) =>
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
            JS.Set("_getTorrentStorage", Callback.Create(Client.GetTorrentStorageNames));
#endif
            if (EnableRecent)
            {
                await LoadRecent(LoadRecentPaused, LoadRecentDeselected);
            }
        }
        /// <summary>
        /// Returns true if the string appears to be a magnet url<br/>
        /// Example: "magnet:?xt=urn:btih:08ada5a7a6183aae1e09d831df6748d566095a10"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsMagnet(string value)
        {
            if (value == null) return false;
            return value.StartsWith(@"magnet:?xt=urn:btih:");// Regex.IsMatch(value, @"^magnet:\?xt=urn:btih:[a-f0-9]{40}\S*$", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// Returns true if the string appears to be a torrent info hash
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInfoHash(string value)
        {
            if (value == null) return false;
            return Regex.IsMatch(value, "^[a-f0-9]{40}$", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// Creates a magnet url based off of a torrent info hash
        /// </summary>
        /// <param name="value"></param>
        /// <param name="addTrackers"></param>
        /// <param name="addDefaultTrackers"></param>
        /// <returns></returns>
        public static string InfoHashToMagnet(string value, IEnumerable<string>? addTrackers = null, bool addDefaultTrackers = true)
        {
            var ret = $"magnet:?xt=urn:btih:{value}&dn:UNKNOWN";
            var trackers = new List<string>();
            if (addDefaultTrackers) trackers.AddRange(PublicTrackers);
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
            // IExtensionFactory services create their extensions for this wire
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
        void Discovery_OnPeer(DiscoveredPeer peer, string source)
        {
            if (Verbose) JS.Log("Discovery_OnPeer", peer, source);
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
        /// <summary>
        /// Releases resources
        /// </summary>
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
        /// <summary>
        /// Destroys all torrents if hasBeenConfirmed == true, returns the number of torrents that will be or were affected
        /// </summary>
        /// <param name="hasBeenConfirmed">Torrents will not actually be destroyed if hasBeenConfirmed != true</param>
        /// <param name="predicate">Allows selecting the torrents to destroy based on a predicate method</param>
        /// <returns></returns>
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
        string[] ImageExtensions { get; set; } = new string[] { ".png", ".jpg", ".jpeg", ".webm" };
        Dictionary<string, string> TorrentPosters = new Dictionary<string, string>();
        bool GetFilenameIsImage(string filename)
        {
            return ImageExtensions.Contains(GetFilenameExtension(filename), StringComparer.OrdinalIgnoreCase);
        }
        string GetFilenameExtension(string filename)
        {
            var pos = filename.LastIndexOf(".");
            return pos == -1 ? "" : filename.Substring(filename.LastIndexOf("."));
        }
        string GetFilenameBase(string filename)
        {
            var pos = filename.LastIndexOf(".");
            return pos == -1 ? filename : filename.Substring(0, pos);
        }
        /// <summary>
        /// Returns a base 64 encoded image that represents the torrents poster image if one can be found and is done being downloaded
        /// </summary>
        /// <param name="torrent"></param>
        /// <returns></returns>
        public async Task<string> GetTorrentPoster(Torrent torrent)
        {
            if (string.IsNullOrEmpty(torrent.InfoHash)) return "";
            if (TorrentPosters.TryGetValue(torrent.InfoHash, out var r)) return r;
            var files = torrent.Files.Using(o => o.ToArray());
            var imageFiles = files.Where(o => GetFilenameIsImage(o.Name)).ToList();
            var posterFile = imageFiles.FirstOrDefault(o => GetFilenameBase(o.Name).Equals("poster"));
            posterFile ??= imageFiles.FirstOrDefault();
            if (posterFile != null)
            {
                if (posterFile.IsDone())
                {
                    using var blob = await posterFile.Blob();
                    var base64Url = await blob.ToDataURLAsync();
                    TorrentPosters[torrent.InfoHash] = base64Url!;
                    files.DisposeAll();
                    return base64Url!;
                }
            }
            files.DisposeAll();
            return "";
        }
        public async Task<File?> GetTorrentPosterFile(Torrent torrent)
        {
            if (string.IsNullOrEmpty(torrent.InfoHash)) return null;
            var files = torrent.Files.Using(o => o.ToArray());
            var imageFiles = files.Where(o => GetFilenameIsImage(o.Name)).ToList();
            var posterFile = imageFiles.FirstOrDefault(o => GetFilenameBase(o.Name).Equals("poster"));
            posterFile ??= imageFiles.FirstOrDefault();
            files.Except(new File[] { posterFile }).ToArray().DisposeAll();
            return posterFile;
        }
    }
}
