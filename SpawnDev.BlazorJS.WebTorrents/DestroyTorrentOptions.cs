using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class DestroyTorrentOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? DestroyStore { get; set; }
    }
}
