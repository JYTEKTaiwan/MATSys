﻿using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;
using System.Text.Json;

public class ParameterCollection : Dictionary<string, object>, IXmlSerializable
{
    public XmlSchema? GetSchema() => null;

    public static ParameterCollection Create(params (string name, object value)[] items)
    {
        var param = new ParameterCollection();
        foreach (var item in items)
        {
            param.Add(item.name, item.value);
        }
        return param;
    }
    public static ParameterCollection Create(IEnumerable< (string name, object value)> items)
    {
        var param = new ParameterCollection();
        foreach (var item in items)
        {
            param.Add(item.name, item.value);
        }
        return param;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
    public void ReadXml(XmlReader reader)
    {
        int cnt = reader.Depth;
        //move to next element
        reader.ReadStartElement();
        do
        {
            var valueTypeString = reader.GetAttribute("Type");
            var t=Type.GetType(valueTypeString);
            reader.MoveToElement();
            var name = reader.Name;            
            XmlSerializer serializer = new XmlSerializer(t, new XmlRootAttribute() { ElementName = name });
            var v = serializer.Deserialize(reader);
            this.Add(name, v);
        } while (cnt != reader.Depth);
        reader.ReadEndElement();



    }

    public void WriteXml(XmlWriter writer)
    {
        foreach (var item in this.AsEnumerable())
        {
            writer.WriteStartElement(item.Key);
            writer.WriteAttributeString("Type", item.Value.GetType().FullName);
            writer.WriteValue(item.Value);
            writer.WriteEndElement();
        }
    }

   
}
