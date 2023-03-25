using SpawnDev.BlazorJS;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class CreateTorrentOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? name { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? comment { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? createdBy { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public DateTime? creationDte { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? filterJunkFiles { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? @private { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? pieceLength { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<List<string>>? announceList { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? urlList { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? info { get; set; } = null;
    }

    public class SeedTorrentOptions : CreateTorrentOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? announce { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Callback? getAnnounceOpts { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? maxWebConns { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? path { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public bool? @private { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public Callback store { get; set; } = null;
    }

    public class AddTorrentOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? announce { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Callback? getAnnounceOpts { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? maxWebConns { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? path { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? @private { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public Callback store { get; set; } = null;
    }
}
