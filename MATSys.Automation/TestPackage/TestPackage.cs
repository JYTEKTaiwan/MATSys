using System.Xml.Serialization;


public class TestPackage
{
    public List<ModuleInfo> Modules { get; set; }=new List<ModuleInfo>();
    public List<TestItem> Items { get; set; }= new List<TestItem>();
    public void Export(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TestPackage));
        var writer = new StreamWriter(path);
        serializer.Serialize(writer, this);
        writer.Close();

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

