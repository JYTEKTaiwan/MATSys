using System.Text;
using MATSys.Factories;
using Microsoft.Extensions.Configuration;

namespace UT_MATSys;

public class UT_DataRecrderFactory
{
    [Test]
    [Category("DataRecorder")]
    public void CreateFromFile()
    {
        var jsonStr = @"{ ""DataRecorder"": {""Type"": ""csv""},
        ""ModulesFolder"": ""C:\\Users\\Way-Develop\\MATSys\\UT_MATSys\\bin\\Debug\\net6.0\\modules"",
        }";
        var ms = new MemoryStream(Encoding.ASCII.GetBytes(jsonStr));
        ConfigurationBuilder cb = new ConfigurationBuilder();
        var config = cb.AddJsonStream(ms).Build();
        ms.Close();
        var fac = new DataRecorderFactory(config);
        var recorder = fac.CreateRecorder(config.GetSection("DataRecorder"));   
        recorder.StartServiceAsync(new CancellationToken());     
        Assert.IsTrue(recorder.Name == "CSVDataRecorder");
    }

}