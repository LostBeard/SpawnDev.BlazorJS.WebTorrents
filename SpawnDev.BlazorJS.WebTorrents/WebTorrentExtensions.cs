using SpawnDev.BlazorJS.JSObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    Console.WriteLine($"Deleting cache for: {entryTorrentName}");
                    await rootDir.RemoveEntry(entry.Name, true);
                    ret.Add(entryTorrentName);
                }
            }
            Console.WriteLine($"ClearTorrentStorage done");
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
                    Console.WriteLine($"Deleting cache for: {entryTorrentName}");
                    await rootDir.RemoveEntry(entry.Name, true);
                    return true;
                }
            }
            return false;
        }
    }
}
