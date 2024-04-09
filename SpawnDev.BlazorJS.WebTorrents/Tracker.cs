using Microsoft.JSInterop;
using System.Security.Cryptography;

namespace SpawnDev.BlazorJS.WebTorrents
{
    public class Tracker : EventEmitter
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Tracker(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Tracker url
        /// </summary>
        public string AnnounceUrl => JSRef.Get<string>("announceUrl");
        /// <summary>
        /// True if the tracker connection has been destroyed
        /// </summary>
        public bool Destroyed => JSRef.Get<bool>("destroyed");
        /// <summary>
        /// Tracker Client
        /// </summary>
        public Client Client => JSRef.Get<Client>("client");
        /// <summary>
        /// Returns the tracker type
        /// </summary>
        public string Type
        {
            get
            {
                if (WebSocketTracker.IsThisTackerType(this)) return nameof(WebSocketTracker);
                if (HTTPTracker.IsThisTackerType(this)) return nameof(HTTPTracker);
                return "";
            }
        }
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
    }
}
