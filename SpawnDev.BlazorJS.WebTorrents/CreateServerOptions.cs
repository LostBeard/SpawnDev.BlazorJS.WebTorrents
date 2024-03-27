using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Used when call WebTorrent.CreateServer
    /// </summary>
    public class CreateServerOptions
    {
        /// <summary>
        /// ServiceWorkerRegistration
        /// </summary>
        public ServiceWorkerRegistration Controller { get; set; }
    }
}
