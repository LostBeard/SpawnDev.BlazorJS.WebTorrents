using Microsoft.JSInterop;
using SpawnDev.BlazorJS;
using System.Text.Json.Serialization;

namespace NetVis
{
    
    public class DataSet : JSObject
    {
        public DataSet(IJSInProcessObjectReference _ref) : base(_ref) { }
        public DataSet(List<VisNode> nodes) : base(JS.New("vis.DataSet", nodes)) { }
        public DataSet(List<VisEdge> edges) : base(JS.New("vis.DataSet", edges)) { }
        public List<string> GetIds() => JSRef.Call<List<string>>("getIds");
        public void Clear() => JSRef.CallVoid("clear");
        public List<string> Add(VisEdge edge) => JSRef.Call<List<string>>("add", edge);
        public List<string> Add(List<VisEdge> edges) => JSRef.Call<List<string>>("add", edges);
        public List<string> Add(VisNode node) => JSRef.Call<List<string>>("add", node);
        public List<string> Add(List<VisNode> nodes) => JSRef.Call<List<string>>("add", nodes);
        public List<string> Update(VisEdge edge) => JSRef.Call<List<string>>("update", edge);
        public List<string> Update(List<VisEdge> edges) => JSRef.Call<List<string>>("update", edges);
        public List<string> Update(VisNode node) => JSRef.Call<List<string>>("update", node);
        public List<string> Update(List<VisNode> nodes) => JSRef.Call<List<string>>("update", nodes);
        public List<string> Remove(List<string> ids) => JSRef.Call<List<string>>("remove", ids);
        public List<string> Remove(string id) => JSRef.Call<List<string>>("remove", id);
        public VisNode GetNode(string id) => JSRef.Call<VisNode>("get", id);
        public VisNode GetEdge(string id) => JSRef.Call<VisNode>("get", id);
    }
}
