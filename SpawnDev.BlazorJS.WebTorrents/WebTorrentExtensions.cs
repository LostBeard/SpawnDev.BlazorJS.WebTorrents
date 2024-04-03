using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Adds extension methods to WebTorrent
    /// </summary>
    public static class WebTorrentExtensions
    {
        static BlazorJSRuntime JS => BlazorJSRuntime.JS;
        /// <summary>
        /// Remove all Torrent data from default Torrent store
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>> ClearTorrentStorage(this WebTorrent _this)
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
#if DEBUG
                    Console.WriteLine($"Deleting cache for: {entryTorrentName}");
#endif
                    await rootDir.RemoveEntry(entry.Name, true);
                    ret.Add(entryTorrentName);
                }
            }
#if DEBUG
            Console.WriteLine($"ClearTorrentStorage done");
#endif
            return ret;
        }
        /// <summary>
        /// Remove Torrent data from default Torrent store
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> ClearTorrentStorage(this WebTorrent _this, string torrentName)
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
#if DEBUG
                    Console.WriteLine($"Deleting cache for: {entryTorrentName}");
#endif
                    await rootDir.RemoveEntry(entry.Name, true);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns a list of torrent names in default storage
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetTorrentStorageNames(this WebTorrent _this)
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
                    ret.Add(entryTorrentName);
                }
            }
            return ret;
        }
        /// <summary>
        /// Returns the torrent from the given torrent instanceId
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="instanceId"></param>
        /// <param name="torrentRet"></param>
        /// <returns></returns>
        public static bool GetTorrentByInstanceId(this WebTorrent _this, string instanceId, out Torrent? torrentRet)
        {
            using var torrents = _this.Torrents;
            foreach (Torrent torrent in torrents)
            {
                if (torrent.InstanceId == instanceId)
                {
                    torrentRet = torrent;
                    return true;
                }
                torrent.Dispose();
            }
            torrentRet = null;
            return false;
        }
        /// <summary>
        /// Returns the torrent and wire from the given wire instanceId
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="instanceId"></param>
        /// <param name="torrentRet"></param>
        /// <param name="wireRet"></param>
        /// <returns></returns>
        public static bool GetWireByInstanceId(this WebTorrent _this, string instanceId, out Torrent? torrentRet, out Wire? wireRet)
        {
            using var torrents = _this.Torrents;
            foreach (Torrent torrent in torrents)
            {
                using var wires = torrent.Wires;
                foreach (Wire wire in wires)
                {
                    if (wire.InstanceId == instanceId)
                    {
                        torrentRet = torrent;
                        wireRet = wire;
                        return true;
                    }
                    wire.Dispose();
                }
                torrent.Dispose();
            }
            torrentRet = null;
            wireRet = null;
            return false;
        }

        /// <summary>
        /// Returns the torrent with the given instanceId<br />
        /// non-spec
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public static Torrent? GetTorrentByInstanceId(this WebTorrent _this, string instanceId)
        {
            using var torrents = _this.Torrents;
            foreach (Torrent torrent in torrents)
            {
                if (torrent.InstanceId == instanceId) return torrent;
                torrent.Dispose();
            }
            return null;
        }
    }
}
