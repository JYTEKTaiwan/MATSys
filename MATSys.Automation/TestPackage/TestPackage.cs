using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace MATSys.Automation;
public class TestPackage
{
    private TestItem currentTestItem;
    public event EventHandler<string> FileExported;
    public event EventHandler FileLoaded;
    public event EventHandler<TestItem> TestItemSelectionChanged;
    private TestPackage template;
    [XmlIgnore] public bool ContentChanged => ValidateContentChanged();
    [XmlArray("Plugins")] public PluginInfoCollection Plugins { get; set; } = new PluginInfoCollection();
    [XmlArray("TestScripts")] public TestItemCollection Items { get; set; } = new TestItemCollection();
    public TestItem CurrentTestItem
    {
        get { return currentTestItem; }
        set
        {
            if (currentTestItem != value)
            {
                currentTestItem = value;
                TestItemSelectionChanged?.Invoke(this, value);
            }
        }
    }
    public void Export(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TestPackage));
        var writer = new StreamWriter(path);
        serializer.Serialize(writer, this);
        writer.Close();
        FileExported?.Invoke(this, path);
    }
    public void LoadFromXML(string path)
    {

        template = DeserializeFromXML<TestPackage>(path);

        var obj = DeserializeFromXML<TestPackage>(path);
        Plugins = obj.Plugins;

        foreach (var item in obj.Items)
        {
            item.Method = string.IsNullOrEmpty(item.Method) ? item.Name : item.Method;
        }
        Items = obj.Items;

        FileLoaded?.Invoke(this, EventArgs.Empty);
    }

    private T DeserializeFromXML<T>(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        var reader = new StreamReader(path);
        var obj = (T)serializer.Deserialize(reader);
        reader.Close();
        return obj;
    }

    private string SerializeToString(TestPackage tp)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TestPackage));
        var sb = new StringBuilder();
        var writer = new StringWriter(sb);
        serializer.Serialize(writer, tp);
        writer.Close();
        return sb.ToString();

    }

    public static TestPackage Load(string path)
    {
        var tp = new TestPackage();
        tp.LoadFromXML(path);
        return tp;
    }

    public bool ValidateContentChanged()
    {
        var str1 = SerializeToString(template);
        var str2 = SerializeToString(this);
        
        return !string.Equals(str1, str2);
    }
}

public class PluginInfoCollection : List<PluginInfo>
{
    public PluginInfoCollection()
    {
    }

    public PluginInfoCollection(IEnumerable<PluginInfo> collection) : base(collection)
    {
    }

    public PluginInfoCollection(int capacity) : base(capacity)
    {
    }

    public PluginInfo this[string alias]=>this.First(x => x.Alias == alias);
    public void Swap(int sourceIndex, int destinationIndex)
    {
        var tmp = this[sourceIndex];
        this[sourceIndex] = this[destinationIndex];
        this[destinationIndex] = tmp;
    }

}


public class TestItemCollection : List<TestItem>
{
    public TestItemCollection()
    {
    }

    public TestItemCollection(IEnumerable<TestItem> collection) : base(collection)
    {
    }

    public TestItemCollection(int capacity) : base(capacity)
    {
    }
    public TestItem this[string name] => this.First(x => x.Name == name);

    public void Swap(int sourceIndex, int destinationIndex)
    {
        var tmp = this[sourceIndex];
        this[sourceIndex] = this[destinationIndex];
        this[destinationIndex] = tmp;
    }

}

public class PluginInfo
{
    [XmlAttribute] public string Alias { get; set; } = "";
    [XmlAttribute] public string Path { get; set; } = "";
    [XmlAttribute] public string QualifiedName { get; set; } = "";

}

