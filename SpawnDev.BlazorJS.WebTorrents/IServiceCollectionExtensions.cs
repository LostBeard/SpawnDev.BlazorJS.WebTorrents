using Microsoft.Extensions.DependencyInjection;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Adds extension methods to IServiceCollection
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add the WebTorrentService and optionally configure on startup
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="configureCallback"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebTorrentService(this IServiceCollection _this, Action<WebTorrentService>? configureCallback = null)
        {
            return _this.AddSingleton(sp =>
            {
                var service = ActivatorUtilities.CreateInstance<WebTorrentService>(sp);
                configureCallback?.Invoke(service);
                return service;
            });
        }
        /// <summary>
        /// Add the WebTorrentService with the provided WebTorrentOptions and optionally configure on startup
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="webTorrentOptions"></param>
        /// <param name="configureCallback"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebTorrentService(this IServiceCollection _this, WebTorrentOptions webTorrentOptions, Action<WebTorrentService>? configureCallback = null)
        {
            return _this.AddSingleton(sp =>
            {
                var service = ActivatorUtilities.CreateInstance<WebTorrentService>(sp);
                service.WebTorrentOptions = webTorrentOptions;
                configureCallback?.Invoke(service);
                return service;
            });
        }
    }
}
