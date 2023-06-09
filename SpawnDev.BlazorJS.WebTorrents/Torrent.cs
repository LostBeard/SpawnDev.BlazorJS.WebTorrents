﻿using Microsoft.JSInterop;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpawnDev.BlazorJS.WebTorrents
{

    // https://github.com/webtorrent/webtorrent/blob/master/docs/api.md#torrent-api
    public class Torrent : JSObject
    {
        public Torrent(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Name of the torrent (string).
        /// </summary>
        public string Name => JSRef.Get<string>("name");
        /// <summary>
        /// Info hash of the torrent (string).
        /// </summary>
        public string InfoHash => JSRef.Get<string>("infoHash");
        /// <summary>
        /// Magnet URI of the torrent (string).
        /// </summary>
        public string MagnetURI => JSRef.Get<string>("magnetURI");
        /// <summary>
        /// .torrent file of the torrent (Uint8Array).
        /// </summary>
        public JSObject TorrentFile => JSRef.Get<JSObject>("torrentFile");
        /// <summary>
        /// .torrent file of the torrent (Blob). Useful for creating Blob URLs via URL.createObjectURL(blob)
        /// </summary>
        public string TorrentFileBlob => JSRef.Get<string>("torrentFileBlob");
        /// <summary>
        /// Array of all tracker servers. Each announce is an URL (string).
        /// </summary>
        public string[] Announce => JSRef.Get<string[]>("announce");
        /// <summary>
        /// Array of all files in the torrent. See documentation for File below to learn what methods/properties files have.
        /// </summary>
        public File[] Files => JSRef.Get<File[]>("files");
        public int FilesLength => JSRef.Get<int>("files.length");
        /// <summary>
        /// Array of all pieces in the torrent. See documentation for Piece below to learn what properties pieces have. Some pieces can be null.
        /// </summary>
        public JSObject Pieces => JSRef.Get<JSObject>("pieces");
        /// <summary>
        /// Length in bytes of every piece but the last one.
        /// </summary>
        public long PieceLength => JSRef.Get<long>("pieceLength");
        /// <summary>
        /// Length in bytes of the last piece (<= of torrent.pieceLength).
        /// </summary>
        public long LastPieceLength => JSRef.Get<long>("lastPieceLength");
        /// <summary>
        /// Time remaining for download to complete (in milliseconds).
        /// </summary>
        public double TimeRemaining => JSRef.Get<double>("timeRemaining");
        /// <summary>
        /// Total bytes received from peers (including invalid data).
        /// </summary>
        public long Received => JSRef.Get<long>("received");
        /// <summary>
        /// Total verified bytes received from peers.
        /// </summary>
        public long Downloaded => JSRef.Get<long>("downloaded");
        /// <summary>
        /// Total bytes uploaded to peers.
        /// </summary>
        public long Uploaded => JSRef.Get<long>("uploaded");
        /// <summary>
        /// Torrent download speed, in bytes/sec.
        /// </summary>
        public double DownloadSpeed => JSRef.Get<double>("downloadSpeed");
        /// <summary>
        /// Torrent upload speed, in bytes/sec.
        /// </summary>
        public double UploadSpeed => JSRef.Get<double>("uploadSpeed");
        /// <summary>
        /// Torrent download progress, from 0 to 1.
        /// </summary>
        public double Progress => JSRef.Get<double>("progress");
        /// <summary>
        /// Torrent "seed ratio" (uploaded / downloaded).
        /// </summary>
        public double Ratio => JSRef.Get<double>("ratio");
        /// <summary>
        /// Number of peers in the torrent swarm.
        /// </summary>
        public int NumPeers => JSRef.Get<int>("numPeers");
        /// <summary>
        /// Max number of simultaneous connections per web seed, as passed in the options.
        /// </summary>
        public int MaxWebConns => JSRef.Get<int>("maxWebConns");
        /// <summary>
        /// Torrent download location.
        /// </summary>
        public string Path => JSRef.Get<string>("path");
        /// <summary>
        /// True when the torrent is ready to be used (i.e. metadata is available and store is ready).
        /// </summary>
        public bool Ready => JSRef.Get<bool>("ready");
        /// <summary>
        /// True when the torrent has stopped connecting to new peers. Note that this does not pause new incoming connections, nor does it pause the streams of existing connections or their wires.
        /// </summary>
        public bool Paused => JSRef.Get<bool>("paused");
        /// <summary>
        /// True when all the torrent files have been downloaded.
        /// </summary>
        public bool Done => JSRef.Get<bool>("done");
        /// <summary>
        /// Sum of the files length (in bytes).
        /// </summary>
        public long Length => JSRef.Get<long>("length");
        /// <summary>
        /// Date of creation of the torrent (as a Date object).
        /// </summary>
        public DateTime Created => JSRef.Get<DateTime>("created");
        /// <summary>
        /// Author of the torrent (string).
        /// </summary>
        public string CreatedBy => JSRef.Get<string>("createdBy");
        /// <summary>
        /// A comment optionnaly set by the author (string).
        /// </summary>
        public string Comment => JSRef.Get<string>("comment");
        /// <summary>
        /// Remove the torrent from its client. Destroy all connections to peers and delete all saved file metadata.
        /// </summary>
        public void Destroy() => JSRef.CallVoid("destroy");
        /// <summary>
        /// Remove the torrent from its client. Destroy all connections to peers and delete all saved file metadata.
        /// </summary>
        /// <param name="callback"></param>
        public void Destroy(ActionCallback callback) => JSRef.CallVoid("destroy", callback);
        /// <summary>
        /// Remove the torrent from its client. Destroy all connections to peers and delete all saved file metadata.
        /// </summary>
        /// <returns></returns>
        public Task DestroyAsync()
        {
            var t = new TaskCompletionSource();
            Destroy(Callback.CreateOne(() => { t.SetResult(); }));
            return t.Task;
        }
        /// <summary>
        /// Selects a range of pieces to prioritize starting with start and ending with end (both inclusive) at the given priority. notify is an optional callback to be called when the selection is updated with new data.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="priority"></param>
        public void Select(int start, int end, long priority = 0) => JSRef.CallVoid("select", start, end, priority);
        /// <summary>
        /// Deprioritizes a range of previously selected pieces.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="priority"></param>
        public void Deselect(int start, int end, int priority = 0) => JSRef.CallVoid("deselect", start, end, priority);
        /// <summary>
        /// Marks a range of pieces as critical priority to be downloaded ASAP. From start to end (both inclusive).
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void Critical(int start, int end) => JSRef.CallVoid("critical", start, end);
        /// <summary>
        /// Temporarily stop connecting to new peers. Note that this does not pause new incoming connections, nor does it pause the streams of existing connections or their wires.
        /// </summary>
        public void Pause() => JSRef.CallVoid("pause");
        /// <summary>
        /// Resume connecting to new peers.
        /// </summary>
        public void Resume() => JSRef.CallVoid("resume");
        // Events
        /// <summary>
        /// Emitted when the info hash of the torrent has been determined.
        /// </summary>
        /// <param name="callback"></param>
        public void OnInfoHash(ActionCallback callback) => JSRef?.CallVoid("on", "infoHash", callback);
        /// <summary>
        /// Emitted when the metadata of the torrent has been determined. This includes the full contents of the .torrent file, including list of files, torrent length, piece hashes, piece length, etc.
        /// </summary>
        /// <param name="callback"></param>
        public void OnMetadata(ActionCallback callback) => JSRef?.CallVoid("on", "metadata", callback);
        /// <summary>
        /// Emitted when the torrent is ready to be used (i.e. metadata is available and store is ready).
        /// </summary>
        /// <param name="callback"></param>
        public void OnReady(ActionCallback callback) => JSRef?.CallVoid("on", "ready", callback);
        /// <summary>
        /// Emitted when there is a warning. This is purely informational and it is not necessary to listen to this event, but it may aid in debugging.
        /// </summary>
        /// <param name="callback"></param>
        public void OnWarning(ActionCallback callback) => JSRef?.CallVoid("on", "warning", callback);
        /// <summary>
        /// Emitted when the torrent encounters a fatal error. The torrent is automatically destroyed and removed from the client when this occurs.
        /// </summary>
        /// <param name="callback"></param>
        public void OnError(ActionCallback callback) => JSRef?.CallVoid("on", "error", callback);
        /// <summary>
        /// Emitted when all the torrent files have been downloaded.
        /// </summary>
        /// <param name="callback"></param>
        public void OnDone(ActionCallback callback) => JSRef?.CallVoid("on", "done", callback);
        /// <summary>
        /// Emitted whenever data is downloaded. Useful for reporting the current torrent status.
        /// </summary>
        /// <param name="callback"></param>
        public void OnDownload(ActionCallback<long> callback) => JSRef?.CallVoid("on", "download", callback);
        /// <summary>
        /// Emitted whenever data is uploaded. Useful for reporting the current torrent status.
        /// </summary>
        /// <param name="callback"></param>
        public void OnUpload(ActionCallback<long> callback) => JSRef?.CallVoid("on", "upload", callback);
        /// <summary>
        /// Emitted whenever a new peer is connected for this torrent. wire is an instance of bittorrent-protocol, which is a node.js-style duplex stream to the remote peer. This event can be used to specify custom BitTorrent protocol extensions.
        /// </summary>
        /// <param name="callback"></param>
        public void OnWire(ActionCallback<Wire> callback) => JSRef?.CallVoid("on", "wire", callback);
        /// <summary>
        /// Emitted every couple of seconds when no peers have been found. announceType is either 'tracker', 'dht', 'lsd', or 'ut_pex' depending on which announce occurred to trigger this event. <br />
        /// Note that if you're attempting to discover peers from a tracker, a DHT, a LSD, and PEX you'll see this event separately for each.
        /// </summary>
        /// <param name="callback"></param>
        public void OnNoPeers(ActionCallback callback) => JSRef?.CallVoid("on", "noPeers", callback);
        public void DeselectAll()
        {
            var files = Files;
            files.ToList().ForEach(o => o.Deselect());
            files.DisposeAll();
        }
        public void DeselectAll(int priority)
        {
            var files = Files;
            files.ToList().ForEach(o => o.Deselect(priority));
            files.DisposeAll();
        }
    }
}
