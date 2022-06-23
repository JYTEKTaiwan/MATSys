using System.Reflection;
using System.Text;
using MATSys.Factories;
using Microsoft.Extensions.Configuration;
using MATSys;

namespace UT_MATSys;

public class UT_DataReocrderFactory
{
    [Test]
    [Category("DataRecorder")]
    public void CreateFromFile()
    {
        Assert.Catch<FileLoadException>(() => 
        {
            var jsonStr = @"{ ""DataRecorder"": {""Type"": ""csv""}
        
        }";
            var ms = new MemoryStream(Encoding.ASCII.GetBytes(jsonStr));
            ConfigurationBuilder cb = new ConfigurationBuilder();
            var config = cb.AddJsonStream(ms).Build();
            ms.Close();
            var fac = new DataRecorderFactory(new DependencyLoader("",""));
            var recorder = fac.CreateRecorder(config.GetSection("DataRecorder"));
            recorder.StartService(new CancellationToken());
            recorder.StopService();

        });
        
    }

}