using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Inherited by the WebTorrent Wire class<br />
    /// node_modules\@types\bittorrent-protocol\index.d.ts<br />
    /// node_modules\@types\node\stream.d.ts<br />
    /// </summary>
    public class Stream : EventEmitter
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Stream(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Is `true` after `'close'` has been emitted.<br />
        /// </summary>
        public bool Destroyed => JSRef.Get<bool>("destroyed");
        public bool Destroying => JSRef.Get<bool>("destroying");
        /// <summary>
        /// Destroy the stream. Optionally emit an `'error'` event, and emit a `'close'`event (unless `emitClose` is set to `false`). After this call, the readable stream will release any internal resources and subsequent calls to `push()`will be ignored.
        /// </summary>
        /// <param name="error"></param>
        public void Destroy(Error? error = null) => JSRef.CallVoid("destroy", error);
        /// <summary>
        /// Emitted when the stream is closed
        /// </summary>
        public JSEventCallback OnClose { get => new JSEventCallback("close", On, RemoveListener); set { } }
    }
}
