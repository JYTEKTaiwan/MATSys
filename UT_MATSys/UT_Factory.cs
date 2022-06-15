using System.Text;
using MATSys.Factories;
using Microsoft.Extensions.Configuration;

namespace UT_MATSys;

public class UT_DataRecrderFactory
{
    [Test]
    public void CreateFromFile()
    {
        var jsonStr = @"{ ""DataRecorder"": {""Type"": ""csv""},
        ""ModulesFolder"": ""..\\..\\..\\..\\examples\\Test\\bin\\Debug\\net6.0\\modules"",
        ""LibraryFolder"": ""..\\..\\..\\..\\examples\\Test\\bin\\Debug\\net6.0\\libs""}";
        var ms = new MemoryStream(Encoding.ASCII.GetBytes(jsonStr));
        ConfigurationBuilder cb = new ConfigurationBuilder();
        var config = cb.AddJsonStream(ms).Build();
        ms.Close();
        var fac = new DataRecorderFactory(config);
        var recorder = fac.CreateRecorder(config.GetSection("DataRecorder"));        
        Assert.IsTrue(recorder.Name == "CSVDataRecorder");
    }

}