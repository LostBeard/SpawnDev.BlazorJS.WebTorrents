using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.JsonConverters;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://raw.githubusercontent.com/webtorrent/webtorrent/master/webtorrent.min.js
    // https://github.com/webtorrent/webtorrent/blob/master/webtorrent.min.js
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md
    // https://github.com/webtorrent/webtorrent/blob/master/CHANGELOG.md
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#webtorrent-api
    // https://github.com/webtorrent/create-torrent#createtorrentinput-opts-function-callback-err-torrent-
    public class WebTorrent : JSObject
    {
        public static bool LibraryIsLoaded => !JS.IsUndefined("WebTorrent");
        public static string LibraryVersion => LibraryIsLoaded ? JS.Get<string>("WebTorrent.VERSION") : "";
        /// <summary>
        /// Returns the web torrent client peerId
        /// </summary>
        public string PeerId => JSRef.Get<string>("peerId");
        public double DownloadSpeed => JSRef.Get<double>("downloadSpeed");
        public double UploadSpeed => JSRef.Get<double>("uploadSpeed");
        public double Progress => JSRef.Get<double>("progress");
        public double Ratio => JSRef.Get<double>("ratio");

        public WebTorrent(IJSInProcessObjectReference _ref) : base(_ref) { }
        public WebTorrent() : base(JS.New("WebTorrent")) { }
        public WebTorrent(WebTorrentOptions options) : base(JS.New("WebTorrent", options)) { }

        public JSEventCallback<Torrent> OnTorrent { get => new JSEventCallback<Torrent>(JSRef, "torrent", "on"); set { } }
        public JSEventCallback<JSObject> OnError { get => new JSEventCallback<JSObject>(JSRef, "error", "on"); set { } }

        public class CreateServerOptions
        {
            public JSObject Controller { get; set; }
        }

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
                var registration = await JS.CallAsync<JSObject?>("navigator.serviceWorker.getRegistration");
                if (registration == null) return false;
                //await JS.CallVoidAsync("navigator.serviceWorker.register", serverScriptPath);
                var controller = await JS.GetAsync<JSObject>("navigator.serviceWorker.ready");
                JSRef!.CallVoid("createServer", new CreateServerOptions { Controller = controller });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR WebTorrent.CreateServer failed: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// Start seeding a new torrent.<br />
        /// </summary>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <param name="onTorrent"></param>
        /// <returns></returns>
        public Torrent Seed(Union<byte[], TypedArray, Blob, IEnumerable<Blob>> input, SeedTorrentOptions? options = null, Action<Torrent>? onTorrent = null) => onTorrent == null ? JSRef!.Call<Torrent>("seed", input, options) : JSRef!.Call<Torrent>("seed", input, options, Callback.CreateOne<Torrent>(onTorrent));
        /// <summary>
        /// Start seeding a new torrent.<br />
        /// </summary>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<Torrent> SeedAsync(Union<byte[], TypedArray, Blob, IEnumerable<Blob>> input, SeedTorrentOptions? options = null)
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
        public Torrent Add(Union<string, byte[], Uint8Array> torrentId, Action<Torrent> callback) => JSRef.Call<Torrent>("add", torrentId, Callback.CreateOne(callback));
        /// <summary>
        /// Start downloading a new torrent.<br />
        /// </summary>
        /// <param name="torrentId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Torrent Add(Union<string, byte[], Uint8Array> torrentId, Action callback) => JSRef.Call<Torrent>("add", torrentId, Callback.CreateOne(callback));
        /// <summary>
        /// Start downloading a new torrent.<br />
        /// </summary>
        /// <param name="torrentId"></param>
        /// <returns></returns>
        public Torrent Add(Union<string, byte[], Uint8Array> torrentId) => JSRef.Call<Torrent>("add", torrentId);
        /// <summary>
        /// Start downloading a new torrent.<br />
        /// </summary>
        /// <param name="torrentId"></param>
        /// <returns></returns>
        public Task<Torrent> AddAsync(Union<string, byte[], Uint8Array> torrentId)
        {
            var tcs = new TaskCompletionSource<Torrent>();
            JSRef.CallVoid("add", torrentId, Callback.CreateOne<Torrent>((torrent) => { tcs.TrySetResult(torrent); }));
            return tcs.Task;
        }
        /// <summary>
        /// Remove a torrent from the client. Destroy all connections to peers and delete all saved file metadata.
        /// </summary>
        /// <param name="torrentId"></param>
        /// <param name="callback"></param>
        /// <returns>Task that completes when the torrent is compeltely removed</returns>
        Task Remove(string torrentId) => JSRef!.CallVoidAsync("remove", torrentId);
    }
}
