using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace MATSys
{
    public interface IService
    {
        string Name { get; }

        void Load(IConfigurationSection section);

        void Load(object configuration);

        void StartService(CancellationToken token);

        JObject Export();

        void StopService();
    }
}
