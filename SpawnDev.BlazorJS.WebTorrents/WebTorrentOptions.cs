using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class WebTorrentOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? maxConns { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? nodeId { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? peerId { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? tracker { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? dht { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? lsd { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? webSeeds { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? utp { get; set; } = null;
    }
}
