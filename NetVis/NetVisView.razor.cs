using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.JSObjects;

namespace NetVis
{
    // https://visjs.org/
    public partial class NetVisView : IDisposable
    {

        [Inject]
        BlazorJSRuntime JS { get; set; }

        [Parameter]
        public string Style { get; set; } = "width: 100%; height: 100%;";

        [Parameter]
        public Func<Task>? UpdateData { get; set; }

        [Parameter]
        public bool UseDemoData { get; set; }

        [Parameter] public EventCallback<NetworkEvent> OnClick { get; set; }
        [Parameter] public EventCallback<NetworkEvent> OnDoubleClick { get; set; }
        [Parameter] public EventCallback<NetworkContextEvent> OnContext { get; set; }

        [Parameter] public EventCallback<NetworkDragEvent> OnDragging { get; set; }
        [Parameter] public EventCallback<NetworkDragEvent> OnDragStart { get; set; }
        [Parameter] public EventCallback<NetworkDragEvent> OnDragEnd { get; set; }

        [Parameter] public EventCallback<NetworkFocusEvent> OnHoverNode { get; set; }
        [Parameter] public EventCallback<NetworkFocusEvent> OnBlurNode { get; set; }

        [Parameter] public EventCallback<NetworkSelectEvent> OnSelect { get; set; }
        [Parameter] public EventCallback<NetworkSelectEvent> OnSelectNode { get; set; }
        [Parameter] public EventCallback<NetworkSelectEvent> OnDeselectNode { get; set; }
        [Parameter] public EventCallback<NetworkSelectEvent> OnSelectEdge { get; set; }
        [Parameter] public EventCallback<NetworkSelectEvent> OnDeselectEdge { get; set; }

        [Parameter] public EventCallback<NetworkZoomEvent> OnZoom { get; set; }

        [Parameter] public EventCallback<string> OnShowPopup { get; set; }
        [Parameter] public EventCallback OnHidePopup { get; set; }

        public ElementReference _container { get; protected set; }

        NetworkOptions Options { get; set; } = new NetworkOptions();

        public Network? Network { get; private set; } = null;

        public NetworkData NetworkData { get; private set; }

        public NodeDataSet Nodes { get; private set; }

        public EdgeDataSet Edges { get; private set; }

        public void Dispose()
        {
            if (Network != null)
            {
                if (OnClick.HasDelegate)
                    Network.OnClick -= Network_OnClick;
                if (OnDoubleClick.HasDelegate)
                    Network.OnDoubleClick -= Network_OnDoubleClick;
                if (OnContext.HasDelegate)
                    Network.OnContext -= Network_OnContext;
                if (OnDragging.HasDelegate)
                    Network.OnDragging -= Network_OnDragging;
                if (OnDragStart.HasDelegate)
                    Network.OnDragStart -= Network_OnDragStart;
                if (OnDragEnd.HasDelegate)
                    Network.OnDragEnd -= Network_OnDragEnd;
                if (OnHoverNode.HasDelegate)
                    Network.OnHoverNode -= Network_OnHoverNode;
                if (OnBlurNode.HasDelegate)
                    Network.OnBlurNode -= Network_OnBlurNode;
                if (OnSelect.HasDelegate)
                    Network.OnSelect -= Network_OnSelect;
                if (OnSelectNode.HasDelegate)
                    Network.OnSelectNode -= Network_OnSelectNode;
                if (OnDeselectNode.HasDelegate)
                    Network.OnDeselectNode -= Network_OnDeselectNode;
                if (OnSelectEdge.HasDelegate)
                    Network.OnSelectEdge -= Network_OnSelectEdge;
                if (OnDeselectEdge.HasDelegate)
                    Network.OnDeselectEdge -= Network_OnDeselectEdge;
                if (OnZoom.HasDelegate)
                    Network.OnZoom -= Network_OnZoom;
                if (OnShowPopup.HasDelegate)
                    Network.OnShowPopup -= Network_OnShowPopup;
                if (OnHidePopup.HasDelegate)
                    Network.OnHidePopup -= Network_OnHidePopup;
            }
            Edges?.Dispose();
            Nodes?.Dispose();
            Network?.Dispose();
        }

        public void Fit() => Network?.Fit();

        public void Clear()
        {
            Nodes?.Clear();
            Edges?.Clear();
        }

        // https://visjs.org/
        // https://visjs.github.io/vis-network/examples/network/data/dynamicData.html
        // https://visjs.github.io/vis-network/examples/network/nodeStyles/circularImages.html

        protected override void OnAfterRender(bool firstRender)
        {
            Console.WriteLine("OnAfterRender");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitAsync();
            }
        }

        void LoadDemoData()
        {
            Nodes.Add(new List<VisNode>
            {
                new VisNode{ Id = "1", Label = "Node 1" },
                new VisNode{ Id = "2", Label = "Node 2" },
                new VisNode{ Id = "3", Label = "Node 3" },
                new VisNode{ Id = "4", Label = "Node 4" },
                new VisNode{ Id = "5", Label = "Node 5" },
            });
            Edges.Add(new List<VisEdge>
            {
                new VisEdge{ From = "1", To = "3" },
                new VisEdge{ From = "1", To = "2" },
                new VisEdge{ From = "2", To = "4" },
                new VisEdge{ From = "2", To = "5" },
                new VisEdge{ From = "3", To = "5" },
            });
        }

        void Network_OnClick(NetworkEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnClick.InvokeAsync(networkEvent);
        }

        void Network_OnDoubleClick(NetworkEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnDoubleClick.InvokeAsync(networkEvent);
        }

        void Network_OnContext(NetworkContextEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnContext.InvokeAsync(networkEvent);
        }

        void Network_OnDragging(NetworkDragEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnDragging.InvokeAsync(networkEvent);
        }
        void Network_OnDragStart(NetworkDragEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnDragStart.InvokeAsync(networkEvent);
        }
        void Network_OnDragEnd(NetworkDragEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnDragEnd.InvokeAsync(networkEvent);
        }

        void Network_OnHoverNode(NetworkFocusEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnHoverNode.InvokeAsync(networkEvent);
        }

        void Network_OnBlurNode(NetworkFocusEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnBlurNode.InvokeAsync(networkEvent);
        }

        void Network_OnSelect(NetworkSelectEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnSelect.InvokeAsync(networkEvent);
        }

        void Network_OnSelectNode(NetworkSelectEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnSelectNode.InvokeAsync(networkEvent);
        }

        void Network_OnDeselectNode(NetworkSelectEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnDeselectNode.InvokeAsync(networkEvent);
        }

        void Network_OnSelectEdge(NetworkSelectEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnSelectEdge.InvokeAsync(networkEvent);
        }

        void Network_OnDeselectEdge(NetworkSelectEvent networkEvent)
        {
            using var domEvent = networkEvent.Event;
            domEvent.PreventDefault();
            _ = OnDeselectEdge.InvokeAsync(networkEvent);
        }

        void Network_OnZoom(NetworkZoomEvent networkEvent)
        {
            _ = OnZoom.InvokeAsync(networkEvent);
        }

        void Network_OnShowPopup(string networkEvent)
        {
            _ = OnShowPopup.InvokeAsync(networkEvent);
        }

        void Network_OnHidePopup()
        {
            _ = OnHidePopup.InvokeAsync();
        }

        async Task InitAsync()
        {
            await JS.LoadScript("_content/NetVis/vis-network.min.js", "vis");
            Nodes = new NodeDataSet();
            Edges = new EdgeDataSet();
            if (UseDemoData) LoadDemoData();
            NetworkData = new NetworkData();
            NetworkData.Nodes = Nodes;
            NetworkData.Edges = Edges;
            //var options = new NetworkOptions();
            Options.AutoResize = true;
            Options.Edges = new NetworkEdgeOptions
            {
                Color = "#dd0000",
                Shadow = true,
            };
            Options.Nodes = new NetworkNodeOptions
            {
                Color = new NodeColorOptions { Border = "#ff0000" },
                Shadow = true,
            };
            Network = new Network(_container, NetworkData, Options);
            if (OnClick.HasDelegate) 
                Network.OnClick += Network_OnClick;
            if (OnDoubleClick.HasDelegate) 
                Network.OnDoubleClick += Network_OnDoubleClick;
            if (OnContext.HasDelegate) 
                Network.OnContext += Network_OnContext;
            if (OnDragging.HasDelegate) 
                Network.OnDragging += Network_OnDragging;
            if (OnDragStart.HasDelegate)
                Network.OnDragStart += Network_OnDragStart;
            if (OnDragEnd.HasDelegate)
                Network.OnDragEnd += Network_OnDragEnd;
            if (OnHoverNode.HasDelegate)
                Network.OnHoverNode += Network_OnHoverNode;
            if (OnBlurNode.HasDelegate)
                Network.OnBlurNode += Network_OnBlurNode;
            if (OnSelect.HasDelegate)
                Network.OnSelect += Network_OnSelect;
            if (OnSelectNode.HasDelegate)
                Network.OnSelectNode += Network_OnSelectNode;
            if (OnDeselectNode.HasDelegate)
                Network.OnDeselectNode += Network_OnDeselectNode;
            if (OnSelectEdge.HasDelegate)
                Network.OnSelectEdge += Network_OnSelectEdge;
            if (OnDeselectEdge.HasDelegate)
                Network.OnDeselectEdge += Network_OnDeselectEdge;
            if (OnZoom.HasDelegate)
                Network.OnZoom += Network_OnZoom;
            if (OnShowPopup.HasDelegate)
                Network.OnShowPopup += Network_OnShowPopup;
            if (OnHidePopup.HasDelegate)
                Network.OnHidePopup += Network_OnHidePopup;

            JS.Set("_network", Network);
            //JS.Set("_nodes", Network);
            //JS.Set("_edges", Network);
            //Network = new Network(_container, NetworkData, Options);
            await UpdateData();
        }

        protected override bool ShouldRender()
        {
            if (Network != null)
            {
                if (UpdateData != null) _ = UpdateData();
            }
            return false;
        }
    }
}
