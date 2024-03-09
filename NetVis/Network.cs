using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.JSObjects;

namespace NetVis
{
    public class NetworkPointerPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class NetworkEventPointer
    {
        public NetworkPointerPoint DOM { get; set; }
        public NetworkPointerPoint Canvas { get; set; }
    }

    public class NetworkContextEvent : NetworkEvent
    {
        public NetworkContextEvent(IJSInProcessObjectReference _ref) : base(_ref) { }
        public MouseEvent DragEvent => JSRef.Get<MouseEvent>("event");
    }

    public class NetworkFocusEvent : NetworkEvent
    {
        public NetworkFocusEvent(IJSInProcessObjectReference _ref) : base(_ref) { }
        public MouseEvent MouseEvent => JSRef.Get<MouseEvent>("event");
    }

    public class NetworkDragEvent : NetworkEvent
    {
        public NetworkDragEvent(IJSInProcessObjectReference _ref) : base(_ref) { }
        public DragEvent DragEvent => JSRef.Get<DragEvent>("event");
    }
    public class NetworkSelectEvent : NetworkEvent
    {
        public NetworkSelectEvent(IJSInProcessObjectReference _ref) : base(_ref) { }
        public DragEvent DragEvent => JSRef.Get<DragEvent>("event");
    }

    public class NetworkZoomEvent
    {
        public string Direction { get; set; } = "";
        public double Scale { get; set; }
        public NetworkPointerPoint Pointer { get; set; }
    }

    public class NetworkEvent : JSObject
    {
        public NetworkEvent(IJSInProcessObjectReference _ref) : base(_ref) { }
        public List<string> Edges => JSRef.Get<List<string>>("edges");
        public Event Event => JSRef.Get<Event>("event");
        public T EventAs<T>() => JSRef.Get<T>("event");
        public JSObject Items => JSRef.Get<JSObject>("items");
        public List<string> Nodes => JSRef.Get<List<string>>("nodes");
        public NetworkEventPointer Pointer => JSRef.Get<NetworkEventPointer>("pointer");
    }

    // Notes
    // Network allows number or string as node id... for simplicity, a string is used in this wrapper
    //
    // References
    // https://visjs.github.io/vis-network/examples/network/events/interactionEvents.html
    public class Network : JSObject
    {
        public Network(IJSInProcessObjectReference _ref) : base(_ref){ }
        public Network(ElementReference container, NetworkData data, NetworkOptions options) : base(JS.New("vis.Network", container, data, options)){ }
        public void Fit() => JSRef.CallVoid("fit");

        public string GetNodeAt(NetworkPointerPoint pointer) => JSRef.Call<string>("getNodeAt", pointer);
        public void SelectNodes(IEnumerable<string> nodeIds) => JSRef.CallVoid("selectNodes", nodeIds);

        public HTMLElement Body => JSRef.Get<HTMLElement>("canvas");
        public HTMLCanvasElement Canvas => JSRef.Get<HTMLCanvasElement>("canvas");

        public JSEventCallback<NetworkEvent> OnClick { get => new JSEventCallback<NetworkEvent>(JSRef, "click", "on", "off"); set { } }
        public JSEventCallback<NetworkEvent> OnDoubleClick { get => new JSEventCallback<NetworkEvent>(JSRef, "doubleClick", "on", "off"); set { } }
        public JSEventCallback<NetworkContextEvent> OnContext { get => new JSEventCallback<NetworkContextEvent>(JSRef, "oncontext", "on", "off"); set { } }

        public JSEventCallback<NetworkDragEvent> OnDragging { get => new JSEventCallback<NetworkDragEvent>(JSRef, "dragging", "on", "off"); set { } }
        public JSEventCallback<NetworkDragEvent> OnDragStart { get => new JSEventCallback<NetworkDragEvent>(JSRef, "dragStart", "on", "off"); set { } }
        public JSEventCallback<NetworkDragEvent> OnDragEnd { get => new JSEventCallback<NetworkDragEvent>(JSRef, "dragEnd", "on", "off"); set { } }

        public JSEventCallback<NetworkFocusEvent> OnHoverNode { get => new JSEventCallback<NetworkFocusEvent>(JSRef, "hoverNode", "on", "off"); set { } }
        public JSEventCallback<NetworkFocusEvent> OnBlurNode { get => new JSEventCallback<NetworkFocusEvent>(JSRef, "blurNode", "on", "off"); set { } }

        public JSEventCallback<NetworkSelectEvent> OnSelect { get => new JSEventCallback<NetworkSelectEvent>(JSRef, "select", "on", "off"); set { } }
        public JSEventCallback<NetworkSelectEvent> OnSelectNode { get => new JSEventCallback<NetworkSelectEvent>(JSRef, "selectNode", "on", "off"); set { } }
        public JSEventCallback<NetworkSelectEvent> OnDeselectNode { get => new JSEventCallback<NetworkSelectEvent>(JSRef, "deselectNode", "on", "off"); set { } }
        public JSEventCallback<NetworkSelectEvent> OnSelectEdge { get => new JSEventCallback<NetworkSelectEvent>(JSRef, "selectEdge", "on", "off"); set { } }
        public JSEventCallback<NetworkSelectEvent> OnDeselectEdge { get => new JSEventCallback<NetworkSelectEvent>(JSRef, "deselectEdge", "on", "off"); set { } }

        public JSEventCallback<NetworkZoomEvent> OnZoom { get => new JSEventCallback<NetworkZoomEvent>(JSRef, "zoom", "on", "off"); set { } }

        public JSEventCallback<string> OnShowPopup { get => new JSEventCallback<string>(JSRef, "showPopup", "on", "off"); set { } }
        public JSEventCallback OnHidePopup { get => new JSEventCallback(JSRef, "hidePopup", "on", "off"); set { } }
    }
}
