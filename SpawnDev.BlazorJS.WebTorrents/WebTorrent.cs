using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://raw.githubusercontent.com/webtorrent/webtorrent/master/webtorrent.min.js
    // https://github.com/webtorrent/webtorrent/blob/master/webtorrent.min.js
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md
    // https://github.com/webtorrent/webtorrent/blob/master/CHANGELOG.md
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#webtorrent-api
    // https://github.com/webtorrent/create-torrent#createtorrentinput-opts-function-callback-err-torrent-
    /// <summary>
    /// WebTorrent is a streaming torrent client for Node.js and the web. WebTorrent provides the same API in both environments.<br />
    /// To use WebTorrent in the browser, WebRTC support is required (Chrome, Firefox, Opera, Safari).<br />
    /// https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#webtorrent-api
    /// </summary>
    public class WebTorrent : EventEmitter
    {
        /// <summary>
        /// Removes the debug level flag used by WebTorrent<br />
        /// https://github.com/webtorrent/webtorrent/blob/master/README.md#enable-debug-logs
        /// </summary>
        public static void RemoveDebugLevel() => JS.CallVoid("localStorage.removeItem", "debug");
        /// <summary>
        /// Sets the debug level flag used by WebTorrent<br />
        /// https://github.com/webtorrent/webtorrent/blob/master/README.md#enable-debug-logs
        /// </summary>
        /// <param name="level"></param>
        public static void SetDebugLevel(string level = "*") => JS.CallVoid("localStorage.setItem", "debug", level);
        /// <summary>
        /// Location of the included WebTorrent library
        /// </summary>
        public static string LatestVersionSrc { get; } = $"./_content/SpawnDev.BlazorJS.WebTorrents/webtorrent.min.js";
        /// <summary>
        /// Imports WebTorrent library from the given location or the default one included with the package
        /// </summary>
        /// <param name="webTorrentJsUrl"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task ImportWebTorrent(string? webTorrentJsUrl = null)
        {
            webTorrentJsUrl = webTorrentJsUrl ?? LatestVersionSrc;
            var isWebTorrentUndefined = JS.IsUndefined("WebTorrent");
            if (!isWebTorrentUndefined) return;
            var WebTorrentModule = await JS.Import(LatestVersionSrc);
            if (WebTorrentModule == null) throw new Exception("WebTorrentService could not be initialized.");
            var WebTorrentClass = WebTorrentModule.GetExport<Function>("default");
            // set WebTorrent on the global scope so it can be used globally
            JS.Set("WebTorrent", WebTorrentClass);
        }
        /// <summary>
        /// Returns true if WebTorrent is not undefined
        /// </summary>
        public static bool LibraryIsLoaded => !JS.IsUndefined("WebTorrent");
        /// <summary>
        /// Returns teh value from WebTorrent.VERSION
        /// </summary>
        public static string LibraryVersion => LibraryIsLoaded ? JS.Get<string>("WebTorrent.VERSION") : "";
        /// <summary>
        /// Returns the web torrent client peerId
        /// </summary>
        public string PeerId => JSRef.Get<string>("peerId");
        /// <summary>
        /// Total download speed for all torrents, in bytes/sec.
        /// </summary>
        public double DownloadSpeed => JSRef.Get<double>("downloadSpeed");
        /// <summary>
        /// Total upload speed for all torrents, in bytes/sec.
        /// </summary>
        public double UploadSpeed => JSRef.Get<double>("uploadSpeed");
        /// <summary>
        /// Total download progress for all active torrents, from 0 to 1.
        /// </summary>
        public double Progress => JSRef.Get<double>("progress");
        /// <summary>
        /// Aggregate "seed ratio" for all torrents (uploaded / downloaded).
        /// </summary>
        public double Ratio => JSRef.Get<double>("ratio");
        /// <summary>
        /// An array of all torrents in the client.
        /// </summary>
        public Array<Torrent> Torrents => JSRef.Get<Array<Torrent>>("torrents");
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public WebTorrent(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Create a new WebTorrent instance.
        /// </summary>
        public WebTorrent() : base(JS.New("WebTorrent")) { }
        /// <summary>
        /// Tracker options that will be used for by added torrents<br />
        /// </summary>
        public TrackerClientOptions Tracker { get => JSRef.Get<TrackerClientOptions>("tracker"); set => JSRef.Set("tracker", value); }
        /// <summary>
        /// Create a new WebTorrent instance.
        /// </summary>
        /// <param name="options"></param>
        public WebTorrent(WebTorrentOptions options) : base(JS.New("WebTorrent", options)) { }
        /// <summary>
        /// Emitted when a torrent is ready to be used (i.e. metadata is available and store is ready). See the torrent section for more info on what methods a torrent has.
        /// </summary>
        public JSEventCallback<Torrent> OnTorrent { get => new JSEventCallback<Torrent>("torrent", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when the client encounters a fatal error. The client is automatically destroyed and all torrents are removed and cleaned up when this occurs.
        /// </summary>
        public JSEventCallback<JSObject> OnError { get => new JSEventCallback<JSObject>("error", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when a torrent is added to client.torrents. This allows attaching to torrent events that may be emitted before the client 'torrent' event is emitted. See the torrent section for more info on what methods a `torrent` has.
        /// </summary>
        public JSEventCallback<Torrent> OnAdd { get => new JSEventCallback<Torrent>("add", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when a torrent is removed from client.torrents. See the torrent section for more info on what methods a `torrent` has.
        /// </summary>
        public JSEventCallback<Torrent> OnRemove { get => new JSEventCallback<Torrent>("remove", On, RemoveListener); set { } }
        /// <summary>
        /// Starts the WebTorrent service worker web server to enable streaming torrents to the document<br />
        /// NOTE: Requires registering a service-worker.js script in the app's index.html and adding the below line<br /><br />
        /// self.importScripts('_content/SpawnDev.BlazorJS.WebTorrents/sw.min.js');
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CreateServer()
        {
            try
            {
                using var navigator = JS.Get<Navigator>("navigator");
                using var serviceWorker = navigator.ServiceWorker;
                if (serviceWorker == null) return false;
                using var reg = await serviceWorker.Ready;
                if (reg == null) return false;
                var options = new CreateServerOptions { Controller = reg };
                CreateServer(options);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR WebTorrent.CreateServer failed: {ex.Message}");
            }
            return false;
        }
        /// <summary>
        /// Creates an http server to serve the contents of this torrent,<br />
        /// dynamically fetching the needed torrent pieces to satisfy http requests.<br />
        /// Range requests are supported.
        /// </summary>
        /// <param name="options"></param>
        public void CreateServer(CreateServerOptions options) => JSRef!.CallVoid("createServer", options);
        /// <summary>
        /// Start seeding a new torrent.<br />
        /// </summary>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <param name="onTorrent"></param>
        /// <returns></returns>
        public Torrent Seed(Union<byte[], TypedArray, Blob, IEnumerable<byte[]>, IEnumerable<TypedArray>, IEnumerable<Blob>> input, SeedTorrentOptions? options = null, Action<Torrent>? onTorrent = null) => onTorrent == null ? JSRef!.Call<Torrent>("seed", input, options) : JSRef!.Call<Torrent>("seed", input, options, Callback.CreateOne<Torrent>(onTorrent));
        /// <summary>
        /// Start seeding a new torrent.<br />
        /// </summary>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<Torrent> SeedAsync(Union<byte[], TypedArray, Blob, IEnumerable<byte[]>, IEnumerable<TypedArray>, IEnumerable<Blob>> input, SeedTorrentOptions? options = null)
        {
            var tcs = new TaskCompletionSource<Torrent>();
            JSRef!.CallVoid("seed", input, options, Callback.CreateOne<Torrent>(torrent => tcs.TrySetResult(torrent)));
            return tcs.Task;
        }
        /// <summary>
        /// Returns the torrent with the given torrentId. Convenience method. Easier than searching through the client.torrents array. Returns null if no matching torrent found.
        /// </summary>
        /// <param name="torrentId"></param>
        /// <returns></returns>
        public Task<Torrent?> Get(string torrentId) => JSRef.CallAsync<Torrent?>("get", torrentId);
        /// <summary>
        /// Start downloading a new torrent.<br />
        /// </summary>
        /// <param name="torrentId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Torrent Add(Union<string, byte[], Uint8Array> torrentId, Action<Torrent> callback, AddTorrentOptions? options = null) => JSRef.Call<Torrent>("add", torrentId, Callback.CreateOne(callback), options);
        /// <summary>
        /// Start downloading a new torrent.<br />
        /// </summary>
        /// <param name="torrentId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Torrent Add(Union<string, byte[], Uint8Array> torrentId, Action callback, AddTorrentOptions? options = null) => JSRef.Call<Torrent>("add", torrentId, Callback.CreateOne(callback), options);
        /// <summary>
        /// Start downloading a new torrent.<br />
        /// </summary>
        /// <param name="torrentId"></param>
        /// <returns></returns>
        public Torrent Add(Union<string, byte[], Uint8Array> torrentId, AddTorrentOptions? options = null) =>
            options == null ? JSRef.Call<Torrent>("add", torrentId) : JSRef.Call<Torrent>("add", torrentId, options);
        /// <summary>
        /// Start downloading a new torrent.<br />
        /// </summary>
        /// <param name="torrentId"></param>
        /// <returns></returns>
        public Task<Torrent> AddAsync(Union<string, byte[], Uint8Array> torrentId, AddTorrentOptions? options = null)
        {
            var tcs = new TaskCompletionSource<Torrent>();
            JSRef!.CallVoid("add", torrentId, Callback.CreateOne<Torrent>((torrent) => { tcs.TrySetResult(torrent); }), options);
            return tcs.Task;
        }
        /// <summary>
        /// Remove a torrent from the client. Destroy all connections to peers and delete all saved file metadata.
        /// </summary>
        /// <param name="torrentId"></param>
        /// <returns>Task that completes when the torrent is completely removed</returns>
        public Task Remove(string torrentId) => JSRef!.CallVoidAsync("remove", torrentId);
        public Task Remove(string torrentId, DestroyTorrentOptions options) => JSRef!.CallVoidAsync("remove", torrentId, options);
        /// <summary>
        /// Destroy the client, including all torrents and connections to peers. If callback is specified, it will be called when the client has gracefully closed.
        /// </summary>
        public void Destroy() => JSRef.CallVoid("destroy");
        /// <summary>
        /// Destroy the client, including all torrents and connections to peers. If callback is specified, it will be called when the client has gracefully closed.
        /// </summary>
        /// <param name="callback"></param>
        public void Destroy(ActionCallback callback) => JSRef.CallVoid("destroy", callback);
        /// <summary>
        /// Remove the torrent from its client. Destroy all connections to peers and delete all saved file metadata.
        /// </summary>
        /// <returns></returns>
        public Task DestroyAsync()
        {
            var t = new TaskCompletionSource();
            Destroy(Callback.CreateOne(t.SetResult));
            return t.Task;
        }
        /// <summary>
        /// Set global upload throttle rate
        /// </summary>
        /// <param name="rate">rate (must be bigger or equal than zero, or -1 to disable throttling)</param>
        public void ThrottleUpload(float rate) => JSRef.CallVoid("throttleUpload", rate);
        /// <summary>
        /// Set global download throttle rate.
        /// </summary>
        /// <param name="rate">rate (must be bigger or equal than zero, or -1 to disable throttling)</param>
        public void ThrottleDownload(float rate) => JSRef.CallVoid("throttleDownload", rate);
    }
}
