

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
        Assert.Catch<InvalidOperationException>(() =>
        {
            try
            {
                var host = Host.CreateDefaultBuilder()
                .ConfigureServices(s => s.AddMATSysService());

                var app=host.Build();
                app.RunAsync();
                var mod = app.Services.GetModule("Dev1");
                app.StopAsync();
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
        Assert.Catch<InvalidOperationException>(() =>
        {
            var host = Host.CreateDefaultBuilder()
                            .ConfigureAppConfiguration(configHost =>
                            {
                                configHost.Sources.Clear();
                                configHost.SetBasePath(Directory.GetCurrentDirectory());
                                configHost.AddJsonFile("UT_Hosting_Modules.json");
                            });
            host.ConfigureServices(s => s.AddMATSysService());

            var app =host.Build();
            app.RunAsync();
            var mod = app.Services.GetModule("Dev1");
            app.StopAsync();
        });

    }
    [Test]
    [Category("MATSys_Scripts")]
    public void AppSettingFileNotFOund()
    {
        Assert.Catch<FileNotFoundException>(() =>
        {
            var host = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(configHost =>
        {
            configHost.Sources.Clear();
            configHost.SetBasePath(Directory.GetCurrentDirectory());
            configHost.AddJsonFile("notfound.json");
        });
            host.ConfigureServices(s => s.AddMATSysService());

            var app = host.Build();
            app.RunAsync();
            app.StopAsync();
        });


    }

}
