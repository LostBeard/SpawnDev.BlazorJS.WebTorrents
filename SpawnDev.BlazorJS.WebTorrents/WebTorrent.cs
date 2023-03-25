using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JsonConverters;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://raw.githubusercontent.com/webtorrent/webtorrent/master/webtorrent.min.js
    // https://github.com/webtorrent/webtorrent/blob/master/webtorrent.min.js
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md
    // https://github.com/webtorrent/webtorrent/blob/master/CHANGELOG.md
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#webtorrent-api
    public class WebTorrent : JSObject
    {
        public static bool LibraryIsLoaded => !JS.IsUndefined("WebTorrent");
        public static string LibraryVersion => LibraryIsLoaded ? JS.Get<string>("WebTorrent.VERSION") : "";
        public double DownloadSpeed => JSRef.Get<double>("downloadSpeed");
        public double UploadSpeed => JSRef.Get<double>("uploadSpeed");
        public double Progress => JSRef.Get<double>("progress");
        public double Ratio => JSRef.Get<double>("ratio");
        public WebTorrent(IJSInProcessObjectReference _ref) : base(_ref) { }
        public WebTorrent() : base(JS.New("WebTorrent")) { }
        public WebTorrent(WebTorrentOptions options) : base(JS.New("WebTorrent", options)) { }

        public void OnTorrent(ActionCallback<Torrent> callback) => JSRef?.CallVoid("on", "torrent", callback);
        public void OnError(ActionCallback<JSObject> callback) => JSRef?.CallVoid("on", "error", callback);

        public class CreateServerOptions
        {
            public JSObject Controller { get; set; }
        }

        /// <summary>
        /// Starts the WebTorrent service worker web server to enable streaming torrents to the document<br />
        /// NOTE: Requires registering a service-worker.js script in the app's index.html and adding the below line<br /><br />
        /// self.importScripts('_content/SpawnDev.BlazorJS.WebTorrents/sw.min.js');
        /// </summary>
        /// <param name="serverScriptPath"></param>
        /// <returns></returns>
        public async Task<bool> CreateServer()
        {
            try
            {
                var registration = await JS.CallAsync<JSObject?>("navigator.serviceWorker.getRegistration", "/");
                if (registration == null) return false;
                //await JS.CallVoidAsync("navigator.serviceWorker.register", serverScriptPath);
                var controller = await JS.GetAsync<JSObject>("navigator.serviceWorker.ready");
                JSRef.CallVoid("createServer", new CreateServerOptions { Controller = controller });
                return true;
            }
            catch { }
            return false;
        }

        public Task<Torrent> Seed(SpawnDev.BlazorJS.JSObjects.File file, SeedTorrentOptions options = null)
        {
            var t = new TaskCompletionSource<Torrent>();
            JSRef.CallVoid("seed", file, options, Callback.CreateOne<Torrent>((s) => t.TrySetResult(s)));
            return t.Task;
        }

        public Task<Torrent> Seed(List<SpawnDev.BlazorJS.JSObjects.File> files, SeedTorrentOptions options = null)
        {
            var t = new TaskCompletionSource<Torrent>();
            JSRef.CallVoid("seed", files, options, Callback.CreateOne<Torrent>((s) => t.TrySetResult(s)));
            return t.Task;
        }

        public Task<Torrent?> Get(string torrentId) => JSRef.CallAsync<Torrent?>("get", torrentId);
        public Torrent Add(string torrentId, ActionCallback<Torrent> callback) => JSRef.Call<Torrent>("add", torrentId, callback);
        public Torrent Add(string torrentId, ActionCallback callback) => JSRef.Call<Torrent>("add", torrentId, callback);
        public Torrent Add(string torrentId) => JSRef.Call<Torrent>("add", torrentId);
        public void AddVoid(string torrentId, ActionCallback<Torrent> callback) => JSRef.Call<Torrent>("add", torrentId, callback);

        public Task<Torrent> AddAsync(string torrentId)
        {
            var tcs = new TaskCompletionSource<Torrent>();
            var callbacks = new CallbackGroup();
            using var torrentEarly = JSRef.Call<Torrent>("add", torrentId, Callback.Create<Torrent>((torrent) =>
            {
                callbacks.Dispose();
                tcs.TrySetResult(torrent);
            }, callbacks));
            torrentEarly.OnError(Callback.Create(() =>
            {
                callbacks.Dispose();
                tcs.TrySetException(new Exception());
            }));
            return tcs.Task;
        }
        void Remove(string infoHash, Action? callback = null)
        {
            if (callback == null)
                JSRef.CallVoid("remove", infoHash);
            else
                JSRef.CallVoid("remove", infoHash, Callback.CreateOne(callback));
        }

        public Task RemoveAsync(string torrentId)
        {
            var t = new TaskCompletionSource();
            Remove(torrentId, () => t.SetResult());
            return t.Task;
        }
    }
}
