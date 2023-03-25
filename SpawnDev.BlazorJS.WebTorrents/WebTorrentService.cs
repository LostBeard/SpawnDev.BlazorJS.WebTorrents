using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md
    public class WebTorrentService : IDisposable, IBackgroundService
    {
        public WebTorrent? webtClient { get; private set; } = null;
        public List<string> AnnounceTrackers { get; private set; } = new List<string>();
        public string WebTorrentLibraryVersion { get; private set; } = "";
        BlazorJSRuntime JS;
        Function? WebTorrent { get; set; }
        public bool BeenInit { get; private set; }
        string latestVersionSrc = $"./_content/SpawnDev.BlazorJS.WebTorrents/webtorrent.min.js";
        public bool ServiceWorkerEnabled { get; private set; }
        ModuleNamespaceObject? WebTorrentModule { get; set; } = null;
        CallbackGroup _callbacks = new CallbackGroup();
        public WebTorrentService(BlazorJSRuntime js)
        {
            JS = js;
        }

        public async Task InitAsync()
        {
            if (BeenInit) return;
            BeenInit = true;
            if (IsDisposed) return;
            WebTorrentModule = await JS.Import(latestVersionSrc);
            if (WebTorrentModule == null) throw new Exception("WebTorrentService could not be initialized.");
            WebTorrentLibraryVersion = WebTorrentModule.GetExport<string?>("default.VERSION") ?? "";
            WebTorrent = WebTorrentModule.GetExport<Function>("default");
            // set WebTorrent on the global scope so it can be used globally
            JS.Set("WebTorrent", WebTorrent);
            webtClient = new WebTorrent();
            if (webtClient == null || webtClient.JSRef == null) throw new Exception("WebTorrentService could not be initialized.");
#if DEBUG
            JS.Set("_webtorrent", webtClient);
#endif
            webtClient.OnError(Callback.Create<JSObject>(OnError, _callbacks));
            webtClient.OnTorrent(Callback.Create<Torrent>(OnTorrent, _callbacks));
            // try to start the ServiceWorker server (if possible)
            // requires a registered service worker that imports the sw.min.js script from this project
            ServiceWorkerEnabled = await webtClient.CreateServer();
        }

        void OnTorrent(Torrent torrent)
        {
            JS.Log("OnTorrent", torrent);
            torrent.Dispose();
        }

        void OnError(JSObject error)
        {
            JS.Log("OnError", error);
            error.Dispose();
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

        public async Task<Torrent> SeedFile(FileSystemFileHandle fileHandle)
        {
            if (fileHandle == null) return null;
            using var file = await fileHandle.GetFile();
            return await SeedFile(file);
        }

        public async Task<Torrent> SeedFile(JSObjects.File file, bool markPrivate = false)
        {
            if (webtClient == null) throw new NullReferenceException(nameof(webtClient));
            if (file == null) throw new ArgumentNullException(nameof(webtClient));
            var options = new SeedTorrentOptions
            {
                announceList = new List<List<string>> { AnnounceTrackers },
            };
            if (markPrivate)
            {
                options.@private = true;
            }
            Torrent torrent = await webtClient.Seed(file, options);
            Console.WriteLine($"Torrent: {torrent.MagnetURI}");
            return torrent;
        }

        public async Task<bool> GetTorrentExists(string torrentId)
        {
            if (webtClient == null) return false;
            using var torrent = await webtClient.Get(torrentId);
            return torrent != null;
        }

        public async Task<Torrent?> GetTorrent(string torrentId, bool allowLoad = true)
        {
            if (webtClient == null) throw new NullReferenceException(nameof(webtClient));
            var torrent = await webtClient.Get(torrentId);
            if (torrent != null) return torrent;
            if (torrent == null && !allowLoad) return null;
            Console.WriteLine("adding torrent");
            AddTorrentOptions options = new AddTorrentOptions();
            options.announce = AnnounceTrackers;
            return await webtClient.AddAsync(torrentId);
        }

    }
}
