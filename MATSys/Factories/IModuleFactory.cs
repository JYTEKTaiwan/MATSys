using MATSys.Modules;

namespace MATSys.Factories
{
    public interface IModuleFactory
    {
        IModule CreateDevice(DeviceInformation info);
    }
}