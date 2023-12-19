using MATSys.Factories;
using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace UT_MATSys;

public class UT_DataReocrderFactory
{
    [Test]
    [Category("Recorder")]
    public void CreateFromFile()
    {
        var jsonStr = File.ReadAllText("appsettings_UT_Factory.json");
        var ms = new MemoryStream(Encoding.ASCII.GetBytes(jsonStr));
        ConfigurationBuilder cb = new ConfigurationBuilder();
        var config = cb.AddJsonStream(ms).Build();
        ms.Close();
        var fac = new RecorderFactory();
        var recorder = fac.CreateRecorder(config.GetSection("Dev1:Recorder"));
    }

    [Test]
    [Category("Recorder")]
    public void CreateFromStaticMethod()
    {
        var a = RecorderFactory.CreateNew<CSVRecorder>(null);
        var b = RecorderFactory.CreateNew(typeof(CSVRecorder), null);

        Assert.IsTrue(a != null && b != null);
    }
}