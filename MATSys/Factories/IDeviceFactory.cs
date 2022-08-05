namespace MATSys.Factories
{
    public interface IDeviceFactory
    {
        IDevice CreateDevice(DeviceInformation info);
    }
}