namespace SpawnDev.BlazorJS.WebTorrents.Demo.Services
{
    public class HostPing
    {
        public string HostName { get; set; }
        public int BestPing { get; set; }
        public int FailCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailTotal { get; set; }
        public int SuccessTotal { get; set; }
        public EpochDateTime? Pinged { get; set; }
        public EpochDateTime? LastSuccess { get; set; }
        public EpochDateTime? LastFail { get; set; }
        public int Ping { get; set; }
        public long BlockCount { get; set; }
    }
}
