using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Options for File read methods
    /// </summary>
    public class FileReadOptions
    {
        /// <summary>
        /// Both start and end are inclusive.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? StartByte { get; set; } = null;
        /// <summary>
        /// Both start and end are inclusive.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? EndByte { get; set; } = null;
    }
}
