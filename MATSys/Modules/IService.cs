using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSys.Modules
{
    public interface IService
    {
        string Name { get; }

        void Load(IConfigurationSection section);

        void Load(object configuration);

        void StartService(CancellationToken token);

        void StopService();
    }
}
