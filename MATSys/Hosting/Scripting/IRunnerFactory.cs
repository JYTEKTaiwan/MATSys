using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSys.Hosting.Scripting
{
    internal interface IRunnerFactory
    {
        IRunner CreateRunner(IConfigurationSection section,bool enabled);
        IRunner CreateRunner(Type typs,IConfigurationSection section);

        void Load(IConfigurationSection section);


    }
}
