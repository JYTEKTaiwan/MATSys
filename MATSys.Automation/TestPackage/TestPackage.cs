using System.Reflection.Metadata.Ecma335;
using System.Xml.Serialization;


public class TestPackage
{
    private TestItem currentTestItem;
    public event EventHandler<string> FileExported;
    public event EventHandler FileLoaded;
    public event EventHandler<TestItem> TestItemSelectionChanged;
    public List<ModuleInfo> Modules { get; set; }=new List<ModuleInfo>();
    public List<TestItem> Items { get; set; }= new List<TestItem>();
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
        XmlSerializer serializer = new XmlSerializer(typeof(TestPackage));
        var reader = new StreamReader("test.xml");
        var obj = (TestPackage)serializer.Deserialize(reader);
        reader.Close();
        Modules = obj.Modules;
        Items = obj.Items;
        FileLoaded?.Invoke(this, EventArgs.Empty);
    }

    public static TestPackage Load(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TestPackage));
        var reader = new StreamReader("test.xml");
        var obj = serializer.Deserialize(reader);
        reader.Close();
        return (TestPackage)obj;
    }
}

