using Microsoft.JSInterop;
using SpawnDev.BlazorJS;
using System.Text.Json.Serialization;

namespace NetVis
{
    
    public class NodeDataSet : JSObject
    {
        public VisNode? this[string id]
        {
            get => Get(id);
        }
        public NodeDataSet(IJSInProcessObjectReference _ref) : base(_ref) { }
        public NodeDataSet(List<VisNode> nodes) : base(JS.New("vis.DataSet", nodes)) { }
        public NodeDataSet() : base(JS.New("vis.DataSet", new List<VisNode>())) { }
        public List<string> GetIds() => JSRef.Call<List<string>>("getIds");
        public void Clear() => JSRef.CallVoid("clear");
        public List<string> Add(VisNode node) => JSRef.Call<List<string>>("add", node);
        public List<string> Add(List<VisNode> nodes) => JSRef.Call<List<string>>("add", nodes);
        public List<string> Update(VisNode node) => JSRef.Call<List<string>>("update", node);
        public List<string> Update(List<VisNode> nodes) => JSRef.Call<List<string>>("update", nodes);
        public List<string> Remove(List<string> ids) => JSRef.Call<List<string>>("remove", ids);
        public List<string> Remove(string id) => JSRef.Call<List<string>>("remove", id);
        public VisNode? Get(string id) => JSRef.Call<VisNode>("get", id);
    }
}
