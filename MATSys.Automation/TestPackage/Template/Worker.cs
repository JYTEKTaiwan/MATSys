using System.Data.Common;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

public class Worker
{
    [XmlAttribute]
    public string Alias { get; set; }
    [XmlAttribute]
    public string MethodName { get; set; }
    public ParameterCollection Parameters { get; set; } = new ParameterCollection();
    public ParameterCollection Conditions { get; set; } = new ParameterCollection();

    public override string ToString()
    {
        var jobj = new JsonObject();
        var param1 = new JsonObject();
        foreach (var item in Parameters)
        {
            var gg = item.Value.GetType();
            param1.Add(item.Key, JsonSerializer.SerializeToNode(item.Value));
        }
        var param2 = new JsonObject();
        foreach (var item in Conditions)
        {
            param2.Add(item.Key, JsonSerializer.SerializeToNode(item.Value));
        }
        jobj.Add(MethodName, new JsonArray(param1, param2));
        return jobj.ToJsonString();
    }
}
