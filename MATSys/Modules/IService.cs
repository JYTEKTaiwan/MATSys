using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSys
{
    public interface IService
    {
        string Name { get; }

        void Load(IConfigurationSection section);

        void LoadFromObject(object configuration);

        void StartService(CancellationToken token);

        void StopService();
    }
}
