namespace SpawnDev.BlazorJS.WebTorrents.Demo.Services
{
    public class FetchStatsService : IBackgroundService
    {
        BlazorJSRuntime JS;
        public FetchStats? FetchStats { get; private set; }
        public FetchStatsService(BlazorJSRuntime js)
        {
            JS = js;
            FetchStats = JS.Get<FetchStats>("fetchStats");
        }
    }
}
