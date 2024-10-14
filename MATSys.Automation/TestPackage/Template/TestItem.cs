
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


public class TestItem
{
    [XmlAttribute] public string Name { get; set; } = "";
    [XmlAttribute] public int Order { get; set; } 
    [XmlAttribute] public bool Skip { get; set; }
    [XmlAttribute] public int Retry { get; set; }
    [XmlAttribute] public string Arguments { get; set; } = "";
    [XmlAttribute] public string PluginID { get; set; } = "";
    [XmlAttribute] public string Method { get; set; } = "";
    [XmlElement("Parameters")]public ParameterCollection Parameters { get; set; } = new ParameterCollection();
    [XmlElement("Assertion")] public ParameterCollection Conditions { get; set; } = new ParameterCollection();
    public string Description { get; set; } = "";
    public TestResult? Result { get; set; }
}
