using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.Toolbox;
using System.Text;

namespace SpawnDev.BlazorJS.WebTorrents.WireExtensions
{
    public class RendezvousWireExtension : WireExtension
    {
        public RendezvousWireExtension(Wire wire, string extensionName) : base(wire, extensionName)
        {
            JS.Log("RendezvousWireExtension()", this);
            JS.Set("__RendezvousWireExtension", this);
        }
    }
    public class RendezvousWireExtensionFactory : IWireExtensionFactory, IAsyncBackgroundService
    {
        BlazorJSRuntime JS;
        WebTorrentService _webTorrentService;
        // when the user joins a group a torrent is created and seeded. the torrent peers, when connected to, communicate via a WebTorrent wire signaler extension
        Dictionary<string, Torrent> _groupTorrents = new Dictionary<string, Torrent>();

        /// <summary>
        /// This property will be passed to wire.use() where it will be called<br />
        /// It should return an instance of the wire extension for use by that wire
        /// </summary>
        public FuncCallback<Wire, WireExtension> CreateWireExtension { get; protected set; }
        public List<RendezvousWireExtension> WireExtensions { get; private set; } = new List<RendezvousWireExtension>();

        public string WireExtensionName { get; } = "xenolinguistics";
        BeforeUnloadService BeforeUnloadService;
        HttpClient HttpClient = new HttpClient();

        public RendezvousWireExtensionFactory(BlazorJSRuntime js, WebTorrentService webTorrentService, BeforeUnloadService beforeUnloadService)
        {
            JS = js;
            //PeerNet = peerNet;
            //AppIdentityService = appIdentityService;
            _webTorrentService = webTorrentService;
            CreateWireExtension = new FuncCallback<Wire, WireExtension>(CreateExtension);
            BeforeUnloadService = beforeUnloadService;
            BeforeUnloadService.OnBeforeUnload += BeforeUnloadService_OnBeforeUnload;
            Console.WriteLine($"TestWireExtensionFactory: {webTorrentService.webtClient!.PeerId}");
        }

        public async Task InitAsync()
        {
            //await JoinGroup("i_love_pickles");
            await JoinGroup("i_love_pickles", "with cheese and bagels");
        }

        async Task<Cache> GetDefaultCache()
        {
            return await Cache.OpenCache("defaultCache");
        }

        private void BeforeUnloadService_OnBeforeUnload(OnBeforeUnloadEvent beforeUnloadEvent)
        {
            foreach (var kvp in _groupTorrents)
            {
                var torrent = kvp.Value;
                Console.WriteLine("Destroying group torrent");
                torrent.Destroy(new Torrent.DestroyTorrentOptions { DestroyStore = true });
            }
            _groupTorrents.Clear();
        }

        protected virtual WireExtension CreateExtension(Wire wire)
        {
            Console.WriteLine("CreateExtension");
            var wireExtension = new RendezvousWireExtension(wire, WireExtensionName);
            WireExtensions.Add(wireExtension);
            wireExtension.OnSupportedPeerConnected += WireExtension_OnSupportedPeerConnected;
            wireExtension.OnMessageReceived += WireExtension_OnMessageReceived;
            //wire.ExtendedHandshake.JSRef!.Set($"{WireExtensionName}_peer_id", InstancePublicKey);
            return wireExtension;
        }

        private void WireExtension_OnMessageReceived(WireExtension wireExtension, byte[] msg)
        {
            JS.Log($"WireExtension_OnMessageReceived: {wireExtension.Wire.PeerId}", wireExtension, msg);
        }

        private void WireExtension_OnSupportedPeerConnected(WireExtension wireExtension, WireExtendedHandshakeEvent extendedHandshake)
        {
            JS.Log($"WireExtension_OnSupportedPeerConnected: {wireExtension.Wire.PeerId}", wireExtension, extendedHandshake);
        }

        public List<string> GetGroupsJoined()
        {
            return _groupTorrents.Keys.ToList();
        }

        public async Task JoinGroup(IEnumerable<string> groups)
        {
            foreach (var group in groups)
            {
                await JoinGroup(group);
            }
        }

        public async Task JoinGroup(string group, string metadata = "")
        {
            using var DefaultCache = await GetDefaultCache();
            // create a torrent with the group name and appid 
            // the WebTorrent peers sharing that torrent are in this group
            if (_groupTorrents.ContainsKey(group)) return;
            var torrentFilename = $"/{nameof(RendezvousWireExtension)}/{group}.torrent";
            // DISABLED!!!
            var torrentFile = true ? null : await DefaultCache.ReadBytes(torrentFilename);
            Torrent torrent;
            if (torrentFile != null)
            {
                torrent = await _webTorrentService.webtClient!.AddAsync(torrentFile);
            }
            else
            {
                var tmp = new string(' ', 1);
                using var buf = new Uint8Array(Encoding.UTF8.GetBytes(group + tmp));
                // The torrent's name must be attached to the Uint8Array as the property name
                buf.JSRef!.Set("name", group);
                // this torrent is given to the WebTorrent tracker(s) to locate our app peers in the same groups
                torrent = await _webTorrentService.webtClient!.SeedAsync(buf, new SeedTorrentOptions
                {
                    Name = group,
                    Comment = metadata,
                });
                torrentFile = torrent.TorrentFileBytes;
                await DefaultCache.WriteBytes(torrentFilename, torrentFile);

                JS.Log($"InfoHash: {torrent.InfoHash ?? ""}", group, metadata);

            }
            _groupTorrents[group] = torrent;
            //var announce = torrent.Announce;
            //await ScrapeServer(torrent.InfoHash);
        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string GeneratePeerId() => RandomString(20);

        async Task ScrapeServer(string infoHash)
        {
            var tracker = "https://tracker.openwebtorrent.com";
            var port = "2344";
            var left = "0";
            var downloaded = "100";
            var uploaded = "0";
            var peerId = GeneratePeerId();
            var compact = "1";
            var url = $@"{tracker}/announce?peer_id={peerId}&info_hash={infoHash}&port={port}&left={left}&downloaded={downloaded}&uploaded={uploaded}&compact={compact}";
            Console.WriteLine($"announce: {url}");
            try
            {
                var resp = await HttpClient.GetAsync(url);
                if (resp.IsSuccessStatusCode)
                {
                    using var DefaultCache = await GetDefaultCache();
                    var infoHashAnnounceDataFile = $"announce/{infoHash}.announce";
                    var data = await resp.Content.ReadAsStringAsync();
                    await DefaultCache.WriteText(infoHashAnnounceDataFile, data);

                }
                var nmt = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ScrapeServer failed: {ex.Message}");
            }
        }

        public void LeaveGroup(IEnumerable<string> groups)
        {
            foreach (var group in groups)
            {
                LeaveGroup(group);
            }
        }

        public void LeaveGroup(string group)
        {
            if (!_groupTorrents.TryGetValue(group, out var torrent)) return;
            torrent.Destroy(new Torrent.DestroyTorrentOptions { DestroyStore = true });
            torrent.Dispose();
            _groupTorrents.Remove(group);
        }

        public void SendMessage(string peerId, string msg)
        {
            //throw new NotImplementedException();
        }

        public void SendMessageAsync(string peerId, string msg)
        {
            //throw new NotImplementedException();
        }

        // TODO - maybe instead of below, we leave all groups
        public void SetPeerSearchEnabled(bool enable)
        {
            //throw new NotImplementedException();
        }
    }
}
