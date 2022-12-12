

using MATSys.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace UT_MATSys;

public class UT_Hosting
{
    [Test]
    [Category("MATSys")]
    public void MATSysSectionNotFound()
    {
        Assert.Catch<KeyNotFoundException>(() =>
        {
            try
            {
                var host = Host.CreateDefaultBuilder().UseMATSys()                
                .ConfigureAppConfiguration(configHost =>
                {
                    configHost.Sources.Clear();
                    configHost.SetBasePath(Directory.GetCurrentDirectory());       
                }).Build();
                host.Run();
                host.StopAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        });

    }
    [Test]
    [Category("MATSys_Modules")]
    public void ModulesTypeNotFound()
    {
        Assert.Catch<AggregateException>(() =>
        {
            var host = Host.CreateDefaultBuilder().UseMATSys()
                            .ConfigureAppConfiguration(configHost =>
                            {
                                configHost.Sources.Clear();
                                configHost.SetBasePath(Directory.GetCurrentDirectory());
                                configHost.AddJsonFile("appsettings_UT_Hosting_Module.json");
                            }).Build();
            host.Run();
            host.StopAsync();
        });

    }
    [Test]
    [Category("MATSys_Scripts")]
    public void AppSettingFileNotFOund()
    {
        Assert.Catch<FileNotFoundException>(() =>
        {
            var host = Host.CreateDefaultBuilder().UseMATSys()
        .ConfigureAppConfiguration(configHost =>
        {
            configHost.Sources.Clear();
            configHost.SetBasePath(Directory.GetCurrentDirectory());
            configHost.AddJsonFile("notfound.json");
        }).Build();
            host.Run();
            host.StopAsync();
        });


    }

    [Test]
    [Category("MATSys_Scripts")]
    public void ATSFileNotFound()
    {
        Assert.Catch<IOException>(() => 
        {
            var host = Host.CreateDefaultBuilder().UseMATSys()
        .ConfigureAppConfiguration(configHost =>
        {
            configHost.Sources.Clear();
            configHost.SetBasePath(Directory.GetCurrentDirectory());
            configHost.AddJsonFile("UT_Hosting_Scripts.json");
        }).Build();
            host.Run();
            host.StopAsync();
        });
        

    }

}
