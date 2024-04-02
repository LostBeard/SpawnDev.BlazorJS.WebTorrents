using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Selections class
    /// </summary>
    public class Selections : JSObject
    {
        public Selections(IJSInProcessObjectReference _ref) : base(_ref) { }
        public Array<SelectionItem> Items => JSRef.Get<Array<SelectionItem>>("_items");
        public int Length => JSRef.Get<int>("length");
        // remove, get, swap, insert, clear, sort
    }
}
