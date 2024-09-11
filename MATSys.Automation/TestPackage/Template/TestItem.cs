
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

[XmlRoot]
public class TestItem
{
    [XmlAttribute]    public string Name { get; set; }
    [XmlAttribute]    public int Order { get; set; }
    [XmlAttribute]    public bool Skip { get; set; }
    [XmlAttribute]    public int Retry { get; set; }
    [XmlAttribute]    public string Arguments { get; set; }

    public string Description { get; set; }
    public Worker Worker { get; set; }
    public TestResult? Result { get; set; }
}
