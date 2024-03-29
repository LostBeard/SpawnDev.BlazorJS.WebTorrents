using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.WebTorrents
{
    /// <summary>
    /// EventEmitter class<br />
    /// https://nodejs.org/docs/latest/api/events.html<br />
    /// https://nodejs.org/docs/latest/api/events.html#class-eventemitter
    /// </summary>
    public class EventEmitter : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public EventEmitter(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Returns an array listing the events for which the emitter has registered listeners. The values in the array are strings or Symbols.
        /// </summary>
        /// <returns></returns>
        public List<string> EventNames() => JSRef.Call<List<string>>("eventNames");
        /// <summary>
        /// The EventEmitter instance will emit its own 'newListener' event before a listener is added to its internal array of listeners.<br />
        /// eventName string | Symbol - The name of the event being listened for<br />
        /// listener Function - The event handler function
        /// </summary>
        public JSEventCallback<JSObject, Function> OnNewListener { get => new JSEventCallback<JSObject, Function>("newListener", On, RemoveListener); set { } }
        /// <summary>
        /// Add an event handler
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="callback"></param>
        public void On(string eventName, Callback callback) => JSRef.CallVoid("on", eventName, callback);
        /// <summary>
        /// Add an event handler that will be called at most once
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="callback"></param>
        public void Once(string eventName, Callback callback) => JSRef.CallVoid("once", eventName, callback);
        /// <summary>
        /// Remove an event listener
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="callback"></param>
        public void RemoveListener(string eventName, Callback callback) => JSRef.CallVoid("removeListener", eventName, callback);
        /// <summary>
        /// Remove al event listeners
        /// </summary>
        /// <param name="eventName"></param>
        public void RemoveAllListeners(string eventName) => JSRef.CallVoid("removeAllListeners", eventName);

        // On, Once, and RemoveListener that support using Action with auto reference handling
        static Dictionary<object, CallBackInfo> CallBackInfos { get; } = new Dictionary<object, CallBackInfo>();
        // Action
        /// <summary>
        /// Add an event handler
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public void On(string type, Action listener)
        {
            if (!CallBackInfos.TryGetValue(listener, out CallBackInfo? info))
            {
                info = new CallBackInfo { Callback = new ActionCallback(listener) };
                CallBackInfos[listener] = info;
            }
            info.RefCount++;
            On(type, info.Callback);
        }
        /// <summary>
        /// Add an event handler that will be called at most once
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public void Once(string type, Action listener) => Once(type, new ActionCallback(listener, true));
        /// <summary>
        /// Remove an event 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public void RemoveListener(string type, Action listener)
        {
            if (!CallBackInfos.TryGetValue(listener, out CallBackInfo? info)) return;
            info.RefCount--;
            RemoveListener(type, info.Callback);
            if (info.RefCount <= 0)
            {
                CallBackInfos.Remove(listener);
                info.Callback.Dispose();
            }
        }
        // Action<>
        /// <summary>
        /// Add an event handler
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public void On<T1>(string type, Action<T1> listener)
        {
            if (!CallBackInfos.TryGetValue(listener, out CallBackInfo? info))
            {
                info = new CallBackInfo { Callback = new ActionCallback<T1>(listener) };
                CallBackInfos[listener] = info;
            }
            info.RefCount++;
            On(type, info.Callback);
        }
        /// <summary>
        /// Add an event handler that will be called at most once
        /// </summary>
        public void Once<T1>(string type, Action<T1> listener) => Once(type, new ActionCallback<T1>(listener, true));
        /// <summary>
        /// Remove an event 
        /// </summary>
        public void RemoveListener<T1>(string type, Action<T1> listener)
        {
            if (!CallBackInfos.TryGetValue(listener, out CallBackInfo? info)) return;
            info.RefCount--;
            RemoveListener(type, info.Callback);
            if (info.RefCount <= 0)
            {
                CallBackInfos.Remove(listener);
                info.Callback.Dispose();
            }
        }
        // Action<,>
        /// <summary>
        /// Add an event handler
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public void On<T1, T2>(string type, Action<T1, T2> listener)
        {
            if (!CallBackInfos.TryGetValue(listener, out CallBackInfo? info))
            {
                info = new CallBackInfo { Callback = new ActionCallback<T1, T2>(listener) };
                CallBackInfos[listener] = info;
            }
            info.RefCount++;
            On(type, info.Callback);
        }
        /// <summary>
        /// Add an event handler that will be called at most once
        /// </summary>
        public void Once<T1, T2>(string type, Action<T1, T2> listener) => Once(type, new ActionCallback<T1, T2>(listener, true));
        /// <summary>
        /// Remove an event 
        /// </summary>
        public void RemoveListener<T1, T2>(string type, Action<T1, T2> listener)
        {
            if (!CallBackInfos.TryGetValue(listener, out CallBackInfo? info)) return;
            info.RefCount--;
            RemoveListener(type, info.Callback);
            if (info.RefCount <= 0)
            {
                CallBackInfos.Remove(listener);
                info.Callback.Dispose();
            }
        }
        // ********************************************************************************************************************************
        /// <summary>
        /// Used internally to track the number of calls 
        /// </summary>
        class CallBackInfo
        {
            /// <summary>
            /// AddEventListener call count - RemoveEventListener call count<br />
            /// Callback will be disposed when RefCount == 0
            /// </summary>
            public int RefCount { get; set; }
            /// <summary>
            /// Holds a reference to the callback fo disposing when done using
            /// </summary>
            public Callback Callback { get; set; }
        }
    }
}
