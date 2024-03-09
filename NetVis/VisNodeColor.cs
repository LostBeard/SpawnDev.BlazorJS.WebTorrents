using System.Text.Json.Serialization;

namespace NetVis
{
    public class VisNodeColor
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Background { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Border { get; set; } = null;

    }
}
