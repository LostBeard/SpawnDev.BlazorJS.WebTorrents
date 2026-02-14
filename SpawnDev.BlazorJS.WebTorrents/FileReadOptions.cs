using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Options for File read methods.
    /// WebTorrent's JS API expects { start, end } (both inclusive).
    /// </summary>
    public class FileReadOptions
    {
        /// <summary>
        /// Both start and end are inclusive.
        /// </summary>
        [JsonPropertyName("start")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? StartByte { get; set; } = null;
        /// <summary>
        /// Both start and end are inclusive.
        /// </summary>
        [JsonPropertyName("end")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? EndByte { get; set; } = null;
    }
}
