using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;

public class PluginInfo
{
    [XmlAttribute] public string Path { get; set; }

    [XmlAttribute] public string QualifiedName { get; set; }
}
