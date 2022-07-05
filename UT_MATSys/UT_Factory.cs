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
            var fac = new RecorderFactory(new DependencyLoader(""));
            var recorder = fac.CreateRecorder(config.GetSection("Recorder"));
            recorder.StartService(new CancellationToken());
            recorder.StopService();
        });
    }
}