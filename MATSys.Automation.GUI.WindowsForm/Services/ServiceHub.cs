using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSys.Automation.GUI.WindowsForm.Services
{
    internal class ServiceHub
    {

        public static Lazy<IServiceScope> Instance = new Lazy<IServiceScope>(Create);

        private static IServiceScope Create()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<TestPackage>();
            services.AddSingleton<TestRunner>();
            return services.BuildServiceProvider().CreateScope();
        }
    }
}
