namespace SpawnDev.BlazorJS.WebTorrents.Demo.Services
{
    public class FetchStatsService : IBackgroundService
    {
        BlazorJSRuntime JS;
        System.Timers.Timer timer = new System.Timers.Timer();
        public FetchStats FetchStats { get; private set; }
        public FetchStatsService(BlazorJSRuntime js)
        {
            JS = js;
            FetchStats = JS.Get<FetchStats>("fetchStats");
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 5000;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            //var originPings = FetchStats.OriginPings;
            //JS.Log("Pings", originPings);
        }
    }
}
