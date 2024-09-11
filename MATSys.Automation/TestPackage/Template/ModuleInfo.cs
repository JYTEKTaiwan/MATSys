using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;

public class ModuleInfo
{
    [XmlAttribute] public string Name { get; set; }

    [XmlAttribute] public string Path { get; set; }

    [XmlAttribute] public string QualifiedName { get; set; }
    public PluginInfo Recorder { get; set; } 
     public PluginInfo Notifier { get; set; }
     public PluginInfo Transceiver { get; set; }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        Path = reader.GetAttribute("Path");
        QualifiedName = reader.GetAttribute("QualifiedName");
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Path", Path);
        writer.WriteAttributeString("QualifiedName", QualifiedName);
    }
}
