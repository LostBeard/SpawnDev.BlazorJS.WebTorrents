using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class ExtendedHandshake : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public ExtendedHandshake(IJSInProcessObjectReference _ref) : base(_ref) { }
        public void SetItem(string key, int value) => JSRef.Set(key, value);
        public T GetItem<T>(string key) => JSRef.Get<T>(key);
    }
}
