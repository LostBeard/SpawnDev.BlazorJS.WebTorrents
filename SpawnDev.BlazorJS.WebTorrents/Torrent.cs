using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;
using System.Security.Cryptography;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://github.com/feross/simple-peer
    /// <summary>
    /// WebTorrent Torrent class<br />
    /// https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#torrent-api
    /// </summary>
    public class Torrent : EventEmitter
    {
        /// <summary>
        /// A dictionary containing the latest announce message from each tracker<br />
        /// non-spec custom property added in LostBeard build of WebTorrents to allow tracking swarm data
        /// </summary>
        public Dictionary<string, TrackerAnnounced> Announced => JSRef.Get<Dictionary<string, TrackerAnnounced>>("announced");
        /// <summary>
        /// Returns the property instanceId, setting to a new value if not set
        /// </summary>
        public string InstanceId
        {
            get
            {
                var value = JSRef!.Get<string?>("instanceId");
                if (string.IsNullOrEmpty(value))
                {
                    value = $"{GetType().Name}_{Convert.ToHexString(RandomNumberGenerator.GetBytes(16))}";
                    JSRef!.Set("instanceId", value);
                }
                return value;
            }
        }
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Torrent(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Torrent discovery>br />
        /// Not available until after torrent ready event is emitted
        /// </summary>
        public Discovery Discovery => JSRef.Get<Discovery>("discovery");
        /// <summary>
        /// An array of the torrent's Wire connections
        /// </summary>
        public Array<Wire> Wires => JSRef.Get<Array<Wire>>("wires");
        /// <summary>
        /// Peers (peers underpin wires)
        /// </summary>
        public Peer[] Peers => JSRef.Get<JSObject>("_peers").Using(o => JS.Call<Peer[]>("Object.values", o));
        /// <summary>
        /// Torrent storage
        /// </summary>
        public TorrentStore Store => JSRef.Get<TorrentStore>("store");
        /// <summary>
        /// Name of the torrent (string).
        /// </summary>
        public string Name => JSRef.Get<string>("name");
        /// <summary>
        /// Info hash of the torrent (string).
        /// </summary>
        public string InfoHash => JSRef.Get<string>("infoHash");
        /// <summary>
        /// Magnet URI of the torrent (string).
        /// </summary>
        public string MagnetURI => JSRef.Get<string>("magnetURI");
        /// <summary>
        /// .torrent file of the torrent (Uint8Array).
        /// </summary>
        public Uint8Array TorrentFile => JSRef.Get<Uint8Array>("torrentFile");
        /// <summary>
        /// .torrent file of the torrent (Blob). Useful for creating Blob URLs via URL.createObjectURL(blob)
        /// </summary>
        public Blob TorrentFileBlob => JSRef.Get<Blob>("torrentFileBlob");
        /// <summary>
        /// Array of all tracker servers. Each announce is an URL (string).
        /// </summary>
        public Array<string> Announce => JSRef.Get<Array<string>>("announce");
        /// <summary>
        /// Array of all files in the torrent. See documentation for File below to learn what methods/properties files have.
        /// </summary>
        public Array<File> Files => JSRef.Get<Array<File>>("files");
        /// <summary>
        /// Returns the number of files in the torrent
        /// </summary>
        public int FilesLength => JSRef.Get<int>("files.length");
        /// <summary>
        /// Array of all pieces in the torrent. See documentation for Piece below to learn what properties pieces have. Some pieces can be null.
        /// </summary>
        public Array<Piece?> Pieces => JSRef.Get<Array<Piece?>>("pieces");
        /// <summary>
        /// Length in bytes of every piece but the last one.
        /// </summary>
        public long PieceLength => JSRef.Get<long>("pieceLength");
        /// <summary>
        /// Length in bytes of the last piece (<= of torrent.pieceLength).
        /// </summary>
        public long LastPieceLength => JSRef.Get<long>("lastPieceLength");
        /// <summary>
        /// Time remaining for download to complete (in milliseconds).
        /// </summary>
        public double? TimeRemaining => JSRef.Get<double?>("timeRemaining");
        /// <summary>
        /// Total bytes received from peers (including invalid data).
        /// </summary>
        public long Received => JSRef.Get<long>("received");
        /// <summary>
        /// Total verified bytes received from peers.
        /// </summary>
        public long Downloaded => JSRef.Get<long>("downloaded");
        /// <summary>
        /// Total bytes uploaded to peers.
        /// </summary>
        public long Uploaded => JSRef.Get<long>("uploaded");
        /// <summary>
        /// Torrent download speed, in bytes/sec.
        /// </summary>
        public double DownloadSpeed => JSRef.Get<double>("downloadSpeed");
        /// <summary>
        /// Torrent upload speed, in bytes/sec.
        /// </summary>
        public double UploadSpeed => JSRef.Get<double>("uploadSpeed");
        /// <summary>
        /// Torrent download progress, from 0 to 1.
        /// </summary>
        public double Progress => JSRef.Get<double>("progress");
        /// <summary>
        /// Torrent "seed ratio" (uploaded / downloaded).<br />
        /// This can be null. If null, this property will return 0;
        /// </summary>
        public double Ratio => JSRef.Get<double?>("ratio") ?? 0;
        /// <summary>
        /// Number of peers in the torrent swarm.
        /// </summary>
        public int NumPeers => JSRef.Get<int>("numPeers");
        /// <summary>
        /// Max number of simultaneous connections per web seed, as passed in the options.
        /// </summary>
        public int MaxWebConns => JSRef.Get<int>("maxWebConns");
        /// <summary>
        /// Torrent download location.
        /// </summary>
        public string Path => JSRef.Get<string>("path");
        /// <summary>
        /// True when the torrent is ready to be used (i.e. metadata is available and store is ready).
        /// </summary>
        public bool Ready => JSRef.Get<bool>("ready");
        /// <summary>
        /// True when the torrent has stopped connecting to new peers. Note that this does not pause new incoming connections, nor does it pause the streams of existing connections or their wires.
        /// </summary>
        public bool Paused => JSRef.Get<bool>("paused");
        /// <summary>
        /// True when all the torrent files have been downloaded.
        /// </summary>
        public bool Done => JSRef.Get<bool>("done");
        /// <summary>
        /// Sum of the files length (in bytes).
        /// </summary>
        public long Length => JSRef.Get<long>("length");
        /// <summary>
        /// Date of creation of the torrent (as a Date object).
        /// </summary>
        public DateTime Created => JSRef.Get<DateTime>("created");
        /// <summary>
        /// Author of the torrent (string).
        /// </summary>
        public string CreatedBy => JSRef.Get<string>("createdBy");
        /// <summary>
        /// WebTorrent client
        /// </summary>
        public WebTorrent Client => JSRef.Get<WebTorrent>("client");
        /// <summary>
        /// A comment optionally set by the author (string).
        /// </summary>
        public string Comment => JSRef.Get<string>("comment");
        /// <summary>
        /// Bitfield representing the pieces we already have
        /// </summary>
        public Bitfield? Bitfield => JSRef.Get<Bitfield>("bitfield");
        /// <summary>
        /// Remove the torrent from its client. Destroy all connections to peers and delete all saved file metadata.
        /// </summary>
        public void Destroy(DestroyTorrentOptions options) => JSRef.CallVoid("destroy", options);
        /// <summary>
        /// Remove the torrent from its client. Destroy all connections to peers and delete all saved file metadata.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="callback"></param>
        public void Destroy(DestroyTorrentOptions options, ActionCallback callback) => JSRef.CallVoid("destroy", options, callback);
        /// <summary>
        /// Remove the torrent from its client. Destroy all connections to peers and delete all saved file metadata.
        /// </summary>
        public void Destroy() => JSRef.CallVoid("destroy");
        /// <summary>
        /// Remove the torrent from its client. Destroy all connections to peers and delete all saved file metadata.
        /// </summary>
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
        /// Remove the torrent from its client. Destroy all connections to peers and delete all saved file metadata.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task DestroyAsync(DestroyTorrentOptions options)
        {
            var t = new TaskCompletionSource();
            Destroy(options, Callback.CreateOne(t.SetResult));
            return t.Task;
        }
        /// <summary>
        /// Selected pieces for download
        /// </summary>
        public Selections Selections => JSRef.Get<Selections>("_selections");
        /// <summary>
        /// Selects a range of pieces to prioritize starting with start and ending with end (both inclusive) at the given priority. notify is an optional callback to be called when the selection is updated with new data.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="priority"></param>
        /// <param name="notify"></param>
        public void Select(int start, int end, long priority = 0, Callback? notify = null) => JSRef.CallVoid("select", start, end, priority, notify);
        /// <summary>
        /// Deprioritizes a range of previously selected pieces.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void Deselect(int start, int end) => JSRef.CallVoid("deselect", start, end);
        /// <summary>
        /// Marks a range of pieces as critical priority to be downloaded ASAP. From start to end (both inclusive).
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void Critical(int start, int end) => JSRef.CallVoid("critical", start, end);
        /// <summary>
        /// Temporarily stop connecting to new peers. Note that this does not pause new incoming connections, nor does it pause the streams of existing connections or their wires.
        /// </summary>
        public void Pause() => JSRef.CallVoid("pause");
        /// <summary>
        /// Remove a peer from the torrent swarm. This is advanced functionality. Normally, you should not need to call torrent.removePeer() manually. WebTorrent will automatically remove peers from the torrent swarm when they're slow or don't have pieces that are needed.
        /// </summary>
        /// <param name="peerId"></param>
        public void RemovePeer(string peerId) => JSRef.CallVoid("removePeer", peerId);
        /// <summary>
        /// Add a web seed to the torrent swarm.<br />
        /// Aeb seed servers must have proper CORS (Cross-origin resource sharing) headers so that data can be fetched across domain.
        /// </summary>
        /// <param name="url"></param>
        public void AddWebSeed(string url) => JSRef.CallVoid("addWebSeed", url);
        /// <summary>
        /// Resume connecting to new peers.
        /// </summary>
        public void Resume() => JSRef.CallVoid("resume");
        /// <summary>
        /// Emitted when the info hash of the torrent has been determined.
        /// </summary>
        public JSEventCallback OnInfoHash { get => new JSEventCallback("infohash", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when the metadata of the torrent has been determined. This includes the full contents of the .torrent file, including list of files, torrent length, piece hashes, piece length, etc.
        /// </summary>
        public JSEventCallback OnMetadata { get => new JSEventCallback("metadata", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when the torrent is ready to be used (i.e. metadata is available and store is ready).
        /// </summary>
        public JSEventCallback OnReady { get => new JSEventCallback("ready", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when there is a warning. This is purely informational and it is not necessary to listen to this event, but it may aid in debugging.
        /// </summary>
        public JSEventCallback<JSObject> OnWarning { get => new JSEventCallback<JSObject>("warning", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when the torrent encounters a fatal error. The torrent is automatically destroyed and removed from the client when this occurs.
        /// </summary>
        public JSEventCallback<JSObject?> OnError { get => new JSEventCallback<JSObject?>("error", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when the torrent is closed
        /// </summary>
        public JSEventCallback OnClose { get => new JSEventCallback("close", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when all the torrent files have been downloaded.
        /// </summary>
        public JSEventCallback OnDone { get => new JSEventCallback("done", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted whenever data is downloaded. Useful for reporting the current torrent status.
        /// </summary>
        public JSEventCallback<long> OnDownload { get => new JSEventCallback<long>("download", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted whenever data is uploaded. Useful for reporting the current torrent status.
        /// </summary>
        public JSEventCallback<long> OnUpload { get => new JSEventCallback<long>("upload", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted whenever a new peer is connected for this torrent. wire is an instance of bittorrent-protocol, which is a node.js-style duplex stream to the remote peer. This event can be used to specify custom BitTorrent protocol extensions.
        /// </summary>
        public JSEventCallback<Wire> OnWire { get => new JSEventCallback<Wire>("wire", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted every couple of seconds when no peers have been found. announceType is either 'tracker', 'dht', 'lsd', or 'ut_pex' depending on which announce occurred to trigger this event. <br />
        /// Note that if you're attempting to discover peers from a tracker, a DHT, a LSD, and PEX you'll see this event separately for each.
        /// </summary>
        public JSEventCallback<string> OnNoPeers { get => new JSEventCallback<string>("noPeers", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted every time a piece is verified, the value of the event is the index of the verified piece.<br />
        /// int index
        /// </summary>
        public JSEventCallback<int> OnVerified { get => new JSEventCallback<int>("verified", On, RemoveListener); set { } }
        /// <summary>
        /// Emitted when _gcSelections is called ond there are torrent._selections.length == 0. May repeat
        /// </summary>
        public JSEventCallback OnIdle { get => new JSEventCallback("idle", On, RemoveListener); set { } }
        public JSEventCallback OnInterested { get => new JSEventCallback("interested", On, RemoveListener); set { } }
        public JSEventCallback OnUninterested { get => new JSEventCallback("uninterested", On, RemoveListener); set { } }
        public JSEventCallback OnHotSwap { get => new JSEventCallback("hotswap", On, RemoveListener); set { } }
        public JSEventCallback<TrackerUpdateMessage> OnTrackerAnnounce { get => new JSEventCallback<TrackerUpdateMessage>("trackerAnnounce", On, RemoveListener); set { } }
        public JSEventCallback OnDhtAnnounce { get => new JSEventCallback("dhtAnnounce", On, RemoveListener); set { } }
        public JSEventCallback<JSObject> OnInvalidPeer { get => new JSEventCallback<JSObject>("invalidPeer", On, RemoveListener); set { } }
        public JSEventCallback<JSObject> OnBlockedPeer { get => new JSEventCallback<JSObject>("blockedPeer", On, RemoveListener); set { } }
        public JSEventCallback<JSObject> OnPeer { get => new JSEventCallback<JSObject>("peer", On, RemoveListener); set { } }
        /// <summary>
        /// Deselects all files
        /// </summary>
        public void DeselectAll()
        {
            using var files = Files;
            files.ToArray().UsingEach(x => x.Deselect());
        }
        /// <summary>
        /// Select all files
        /// </summary>
        public void SelectAll()
        {
            using var files = Files;
            files.ToArray().UsingEach(x => x.Select());
        }
        /// <summary>
        /// Returns when the torrent is ready
        /// </summary>
        /// <returns></returns>
        public async Task WhenReady()
        {
            if (Ready) return;
            var tcs = new TaskCompletionSource();
            OnReady += tcs.SetResult;
            await tcs.Task;
            OnReady -= tcs.SetResult;
        }
        /// <summary>
        /// Returns when the torrent is ready or throws if cancelled
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task WhenReady(CancellationToken cancellationToken)
        {
            if (Ready) return;
            var tcs = new TaskCompletionSource(cancellationToken);
            OnReady += tcs.SetResult;
            try
            {
                await tcs.Task;
            }
            finally
            {
                OnReady -= tcs.SetResult;
            }
        }
        /// <summary>
        /// Returns when the torrent is ready or throws if a timeout occurs
        /// </summary>
        /// <param name="timeoutMS"></param>
        /// <returns></returns>
        public async Task WhenReady(int timeoutMS)
        {
            if (Ready) return;
            using var cts = new CancellationTokenSource(timeoutMS);
            var tcs = new TaskCompletionSource(cts);
            OnReady += tcs.SetResult;
            try
            {
                await tcs.Task;
            }
            finally
            {
                OnReady -= tcs.SetResult;
            }
        }
    }
}
