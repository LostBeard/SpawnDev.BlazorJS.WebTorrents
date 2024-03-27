using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.JSObjects;
using Timer = System.Timers.Timer;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Pages
{
    public partial class Index : IDisposable
    {
        [Inject]
        WebTorrentService WebTorrentService { get; set; }

        [Inject]
        BlazorJSRuntime JS { get; set; }

        Dictionary<string, string> knownMagnets = new Dictionary<string, string>   {
            { "Big Buck Bunny", "magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c&dn=Big+Buck+Bunny&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fbig-buck-bunny.torrent"},
            { "Cosmos Laundromat", "magnet:?xt=urn:btih:c9e15763f722f23e98a29decdfae341b98d53056&dn=Cosmos+Laundromat&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fcosmos-laundromat.torrent" },
            { "Sintel", "magnet:?xt=urn:btih:08ada5a7a6183aae1e09d831df6748d566095a10&dn=Sintel&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fsintel.torrent" },
            { "Tears of Steel", "magnet:?xt=urn:btih:209c8226b299b308beaf2b9cd3fb49212dbd13ec&dn=Tears+of+Steel&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Ftears-of-steel.torrent" },
        };

        ElementReference videoElRef { get; set; }
        File[]? TorrentFiles = null;
        File? largestMp4File = null;
        Torrent? Torrent { get; set; }
        string TorrentHash => Torrent == null ? "" : Torrent.InfoHash;
        HTMLVideoElement? videoEl = null;
        string StatusMsg = "";
        string torrentId = "";
        Timer tmr = new Timer();
        private void OnInputEvent(ChangeEventArgs changeEvent)
        {
            torrentId = (string)changeEvent.Value;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                tmr.Interval = 500;
                tmr.Elapsed += Tmr_Elapsed;
                tmr.Enabled = true;
                videoEl = new HTMLVideoElement(JS.ToJSRef(videoElRef));
                StateHasChanged();
                await WebTorrentService.EnableServer();
            }
        }

        private void Tmr_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            StateHasChanged();
        }

        void DisposeTorrent()
        {
            Torrent?.Dispose();
            Torrent = null;
            TorrentFiles?.DisposeAll();
            TorrentFiles = null;
            largestMp4File = null;
        }

        async Task LoadTorrent()
        {
            try
            {
                StatusMsg = "Loading torrent...";
                await LoadTorrent(torrentId);
                StatusMsg = "Torrent loaded";
            }
            catch
            {
                StatusMsg = "Failed to load torrent.";
            }
        }
        async Task LoadTorrent(string torrentIdNew)
        {
            JS.Log($"LoadTorrent", WebTorrentService.ServiceWorkerEnabled);
            try
            {
                if (videoEl == null) return;
                DisposeTorrent();
                torrentId = torrentIdNew;
                Torrent = await WebTorrentService.GetTorrent(torrentId);
                await Torrent!.WhenReady();
                TorrentFiles = Torrent.Files;
                largestMp4File = TorrentFiles.Where(o => o.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)).OrderByDescending(o => o.Length).FirstOrDefault();
                if (largestMp4File != null)
                {
                    if (WebTorrentService.ServiceWorkerEnabled)
                    {
                        //largestMp4File.Select(1);
                        try
                        {
                            videoEl.Muted = true;
                            largestMp4File.StreamTo(videoEl);
                        }
                        catch
                        {
                            // streaming failed. fallback to playing after download...
                            StatusMsg = "Streaming playback failed.";
                        }
                    }
                    else
                    {
                        // streaming not available. fallback to playing after download...
                    }
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                JS.Log($"LoadTorrent failed: {ex.Message}");
            }
        }

        public void Dispose()
        {
            tmr.Dispose();
            videoEl?.Dispose();
            DisposeTorrent();
        }

        public static string ToBytesCount(long bytes)
        {
            int unit = 1024;
            string unitStr = "B";
            if (bytes < unit)
            {
                return string.Format("{0} {1}", bytes, unitStr);
            }
            int exp = (int)(Math.Log(bytes) / Math.Log(unit));
            return string.Format("{0:##.##} {1}{2}", bytes / Math.Pow(unit, exp), "KMGTPEZY"[exp - 1], unitStr);
        }
    }
}
