using MATSys;
using MATSys.Factories;
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
            var jsonStr = @"{ ""Recorder"": {""Type"": ""csv""}
        }";
            var ms = new MemoryStream(Encoding.ASCII.GetBytes(jsonStr));
            ConfigurationBuilder cb = new ConfigurationBuilder();
            var config = cb.AddJsonStream(ms).Build();
            ms.Close();
            var fac = new RecorderFactory(config);
            var recorder = fac.CreateRecorder(config.GetSection("Recorder"));
            recorder.StartService(new CancellationToken());
            recorder.StopService();
        });
    }

    [Test]
    [Category("Recorder")]
    public void CreateFromStaticMethod()
    {
        var a = RecorderFactory.CreateNew<CSVRecorder>(null);
        var b = RecorderFactory.CreateNew(typeof(CSVRecorder),null);

        Assert.IsTrue(a != null&& b!=null);
    }
    internal class CSVRecorder : IRecorder
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