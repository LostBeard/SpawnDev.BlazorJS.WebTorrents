using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// Webtorrent Files closely mimic W3C Files/Blobs except for slice where instead you pass the offsets as objects to the arrayBuffer/stream/createReadStream functions.<br />
    /// https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#file-api
    /// </summary>
    public class File : EventEmitter
    {
        /// <summary>
        /// Returns the property instanceId, setting to a new value if not set
        /// </summary>
        public string InstanceId
        {
            get
            {
                var value = JSRef!.Get<string?>("instanceId");
                if (string.IsNullOrEmpty(value))
                {
                    value = $"{GetType().Name}_{Convert.ToHexString(RandomNumberGenerator.GetBytes(16))}";
                    JSRef!.Set("instanceId", value);
                }
                return value;
            }
        }
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public File(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// File name, as specified by the torrent. Example: 'some-filename.txt'
        /// </summary>
        public string Name => JSRef!.Get<string>("name");
        /// <summary>
        /// File path, as specified by the torrent. Example: 'some-folder/some-filename.txt'
        /// </summary>
        public string Path => JSRef!.Get<string>("path");
        /// <summary>
        /// File length (in bytes), as specified by the torrent. Example: 12345
        /// </summary>
        public long Length => JSRef!.Get<long>("length");
        /// <summary>
        /// File length (in bytes), as specified by the torrent. Example: 12345
        /// </summary>
        public long Size => JSRef!.Get<long>("size");
        /// <summary>
        /// Mime type of the file, falls back to application/octet-stream if the type is not recognized.
        /// </summary>
        public string Type => JSRef!.Get<string>("type");
        /// <summary>
        /// Total verified bytes received from peers, for this file.
        /// </summary>
        public long Downloaded => JSRef!.Get<long>("downloaded");
        /// <summary>
        /// File download progress, from 0 to 1.
        /// </summary>
        public double Progress => JSRef!.Get<double>("progress");
        /// <summary>
        /// Selects the file to be downloaded, at the given priority. Useful if you know you need the file at a later stage.
        /// </summary>
        public void Select() => JSRef!.CallVoid("select");
        /// <summary>
        /// Selects the file to be downloaded, at the given priority. Useful if you know you need the file at a later stage.
        /// </summary>
        public void Select(int priority) => JSRef!.CallVoid("select", priority);
        /// <summary>
        /// Deselects the file's specific priority, which means it won't be downloaded unless someone creates a stream for it.<br />
        /// *Note: This method is currently not working as expected, see dcposch answer on #164 for a nice work around solution.
        /// </summary>
        public void Deselect() => JSRef!.CallVoid("deselect");
        /// <summary>
        /// Deselects the file's specific priority, which means it won't be downloaded unless someone creates a stream for it.<br />
        /// *Note: This method is currently not working as expected, see dcposch answer on #164 for a nice work around solution.
        /// </summary>
        public void Deselect(int priority) => JSRef!.CallVoid("deselect", priority);
        /// <summary>
        /// Create a readable stream to the file. Pieces needed by the stream will be prioritized highly and fetched from the swarm first.
        /// You can pass opts to stream only a slice of a file.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>ReadableStream</returns>
        public ReadableStream CreateReadStream(FileReadOptions options) => JSRef!.Call<ReadableStream>("createReadStream", options);
        /// <summary>
        /// Create a readable stream to the file. Pieces needed by the stream will be prioritized highly and fetched from the swarm first.
        /// You can pass opts to stream only a slice of a file.
        /// </summary>
        /// <returns>ReadableStream</returns>
        public ReadableStream CreateReadStream() => JSRef!.Call<ReadableStream>("createReadStream");
        /// <summary>
        /// Create a readable stream to the file. Pieces needed by the stream will be prioritized highly and fetched from the swarm first.
        /// You can pass opts to stream only a slice of a file.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>ReadableStream</returns>
        public ReadableStream Stream(FileReadOptions options) => JSRef!.Call<ReadableStream>("stream", options);
        /// <summary>
        /// Create a readable stream to the file. Pieces needed by the stream will be prioritized highly and fetched from the swarm first.
        /// You can pass opts to stream only a slice of a file.
        /// </summary>
        /// <returns>ReadableStream</returns>
        public ReadableStream Stream() => JSRef!.Call<ReadableStream>("stream");
        /// <summary>
        /// Create a readable stream to the file. Pieces needed by the stream will be prioritized highly and fetched from the swarm first.
        /// You can pass opts to stream only a slice of a file.
        /// </summary>
        /// <returns>AsyncIterator</returns>
        public AsyncIterator GetIterator() => JSRef!.Call<AsyncIterator>("Symbol.asyncIterator");
        /// <summary>
        /// Create a readable stream to the file. Pieces needed by the stream will be prioritized highly and fetched from the swarm first.
        /// You can pass opts to stream only a slice of a file.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>AsyncIterator</returns>
        public AsyncIterator GetIterator(FileReadOptions options) => JSRef!.Call<AsyncIterator>("Symbol.asyncIterator", options);
        /// <summary>
        /// Create a readable stream to the file. Pieces needed by the stream will be prioritized highly and fetched from the swarm first.
        /// You can pass opts to stream only a slice of a file.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Task<ArrayBuffer></returns>
        public Task<ArrayBuffer> ArrayBuffer(FileReadOptions options) => JSRef!.CallAsync<ArrayBuffer>("arrayBuffer", options);
        /// <summary>
        /// Create a readable stream to the file. Pieces needed by the stream will be prioritized highly and fetched from the swarm first.
        /// You can pass opts to stream only a slice of a file.
        /// </summary>
        /// <returns>Task<ArrayBuffer></returns>
        public Task<ArrayBuffer> ArrayBuffer() => JSRef!.CallAsync<ArrayBuffer>("arrayBuffer");
        /// <summary>
        /// Create a readable stream to the file. Pieces needed by the stream will be prioritized highly and fetched from the swarm first.
        /// You can pass opts to stream only a slice of a file.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Task<Blob></returns>
        public Task<Blob> Blob(FileReadOptions options) => JSRef!.CallAsync<Blob>("blob", options);
        /// <summary>
        /// Create a readable stream to the file. Pieces needed by the stream will be prioritized highly and fetched from the swarm first.
        /// You can pass opts to stream only a slice of a file.
        /// </summary>
        /// <returns>Task&lt;Blob&gt;</returns>
        public Task<Blob> Blob() => JSRef!.CallAsync<Blob>("blob");
        /// <summary>
        /// Returns the entire file as a UTF8-Text using Blob.Text()
        /// </summary>
        /// <returns></returns>
        public async Task<string> Text() => await (await JSRef!.CallAsync<Blob>("blob")).UsingAsync(async o => await o.Text());
        /// <summary>
        /// Requires client.createServer to be ran beforehand. Sets the element source to the file's streaming URL. Supports streaming, seeking and all browser codecs and containers.
        /// </summary>
        /// <param name="elRef"></param>
        public void StreamTo(JSObject elRef) => JSRef!.CallVoid("streamTo", elRef);
        /// <summary>
        /// Requires client.createServer to be ran beforehand. Sets the element source to the file's streaming URL. Supports streaming, seeking and all browser codecs and containers.
        /// </summary>
        /// <param name="elRef"></param>
        public void StreamTo(ElementReference elRef) => JSRef!.CallVoid("streamTo", elRef);
        /// <summary>
        /// Requires client.createServer to be ran beforehand.<br />
        /// Returns the URL of the file which is recognized by the HTTP server.<br />
        /// This method is useful both for servers which run WebTorrent or client apps.
        /// </summary>
        public string StreamURL => JSRef!.Get<string>("streamURL");
        /// <summary>
        /// Emitted when the file has been downloaded.
        /// </summary>
        public JSEventCallback OnDone { get => new JSEventCallback("done", On, RemoveListener); set { } }
    }
}
