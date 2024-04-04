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

        //bestPing: -1,
        //failCount: 0,
        //successCount: 0,
        //failTotal: 0,
        //successTotal: 0,
        //pinged: null,
        //lastSuccess: null,
        //lastFail: null,
        //ping: -1,
        public class OriginPing
        {
            public string Origin { get; set; }
            public int BestPing { get; set; }
            public int FailCount { get; set; }
            public int SuccessCount { get; set; }
            public int FailTotal { get; set; }
            public int SuccessTotal { get; set; }
            public EpochDateTime? Pinged { get; set; }
            public EpochDateTime? LastSuccess { get; set; }
            public EpochDateTime? LastFail { get; set; }
            public int Ping { get; set; }
        }
    }
}
