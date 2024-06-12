using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// WireExtendedHandshakeEvent
    /// </summary>
    public class WireExtendedHandshakeEvent : ExtendedHandshake
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public WireExtendedHandshakeEvent(IJSInProcessObjectReference _ref) : base(_ref) { }

        /// <summary>
        /// m will contain a dictionary where the keys being the supported wire extension names<br />
        /// Dictionary of supported extension messages which maps names of extensions to an extended message ID for each extension message. The only requirement on these IDs is that no extension message share the same one. Setting an extension number to zero means that the extension is not supported/disabled. The client should ignore any extension names it doesn't recognize.
        /// </summary>
        [JsonPropertyName("m")]
        public Dictionary<string, int>? M => JSRef!.Get<Dictionary<string, int>>("m");
        /// <summary>
        /// List of peer supported extensions
        /// </summary>
        public List<string> Extensions => JSRef!.Get<JSObject>("m").JSRef!.GetPropertyNames();
    }
}
