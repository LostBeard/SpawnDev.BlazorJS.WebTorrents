using Microsoft.JSInterop;
using SpawnDev.BlazorJS;
using System.Text.Json.Serialization;

namespace NetVis
{
    
    public class EdgeDataSet : JSObject
    {
        public VisEdge this[string id]
        {
            get => Get(id);
        }
        public EdgeDataSet(IJSInProcessObjectReference _ref) : base(_ref) { }
        public EdgeDataSet(List<VisEdge> edges) : base(JS.New("vis.DataSet", edges)) { }
        public EdgeDataSet() : base(JS.New("vis.DataSet", new List<VisNode>())) { }
        public List<string> GetIds() => JSRef.Call<List<string>>("getIds");
        public void Clear() => JSRef.CallVoid("clear");
        public List<string> Add(VisEdge edge) => JSRef.Call<List<string>>("add", edge);
        public List<string> Add(List<VisEdge> edges) => JSRef.Call<List<string>>("add", edges);
        public List<string> Update(VisEdge edge) => JSRef.Call<List<string>>("update", edge);
        public List<string> Update(List<VisEdge> edges) => JSRef.Call<List<string>>("update", edges);
        public List<string> Remove(List<string> ids) => JSRef.Call<List<string>>("remove", ids);
        public List<string> Remove(string id) => JSRef.Call<List<string>>("remove", id);
        public VisEdge Get(string id) => JSRef.Call<VisEdge>("get", id);
    }
}
