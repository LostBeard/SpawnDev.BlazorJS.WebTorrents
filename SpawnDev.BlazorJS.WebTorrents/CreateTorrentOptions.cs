using SpawnDev.BlazorJS;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    // https://github.com/webtorrent/create-torrent#createtorrentinput-opts-function-callback-err-torrent-
    public class CreateTorrentOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Comment { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CreatedBy { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public DateTime? creationDte { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? FilterJunkFiles { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Private { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PieceLength { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<List<string>>? AnnounceList { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? UrlList { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Info { get; set; } = null;
    }


    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#clientaddtorrentid-opts-function-ontorrent-torrent-
    public class SeedTorrentOptions : CreateTorrentOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Announce { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Callback? GetAnnounceOpts { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxWebConns { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public string? Path { get; set; } = null;
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public Callback Store { get; set; } = null;
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
