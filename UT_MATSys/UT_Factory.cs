using MATSys.Factories;
using MATSys.Modules;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace UT_MATSys;

public class UT_DataReocrderFactory
{
    [Test]
    [Category("Recorder")]
    public void CreateFromFile()
    {
        Assert.Catch<FileNotFoundException>(() =>
        {
            var jsonStr = File.ReadAllText("appsettings.json");
            var ms = new MemoryStream(Encoding.ASCII.GetBytes(jsonStr));
            ConfigurationBuilder cb = new ConfigurationBuilder();
            var config = cb.AddJsonStream(ms).Build();
            ms.Close();
            var fac = new RecorderFactory(config);
            var recorder = fac.CreateRecorder(config.GetSection("Dev1:Recorder"));
            recorder.StartService(new CancellationToken());
            recorder.StopService();
        });
    }

    [Test]
    [Category("Recorder")]
    public void CreateFromStaticMethod()
    {
        var a = RecorderFactory.CreateNew<CSVRecorder2>(null);
        var b = RecorderFactory.CreateNew(typeof(CSVRecorder2),null);

        Assert.IsTrue(a != null&& b!=null);
    }
    internal class CSVRecorder2 : IRecorder
    {
        public string Name => "";

        public void Load(IConfigurationSection section)
        {
        }

        public void Load(object configuration)
        {
        }

        public void StartService(CancellationToken token)
        {
        }

        public void StopService()
        {
        }

        public void Write(object data)
        {
        }

        public Task WriteAsync(object data)
        {
            return Task.CompletedTask;
        }
    }
}