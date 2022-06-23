using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MATSys.Factories
{
    public sealed class DeviceFactory : IDeviceFactory
    {
        private readonly DependencyLoader _loader;
        private readonly IServiceProvider _services;
        public IEnumerable<DeviceInformation> DeviceInfos { get; } 

        public DeviceFactory(IServiceProvider services)
        {            
            _services = services;
            _loader = services.GetRequiredService<DependencyLoader>();            
            DeviceInfos = _loader.ListDevices();
        }
    
        public IDevice CreateDevice(DeviceInformation info)
        {
            return (IDevice)Activator.CreateInstance(info.DeviceType!, new object[] { _services, info.Name })!;
        }
    }
}