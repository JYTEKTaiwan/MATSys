using MATSys.Factories;
using MATSys.Hosting;
using MATSys.Plugins;

namespace UT_MATSys;
public class UT_Export
{

    [Test]
    public void ExportNetMQTransceiver()
    {
        NetMQTransceiverConfiguration config = new NetMQTransceiverConfiguration();
        config.LocalIP = "inproc://127.0.0.1";
        config.Port = 1224;

        var tran = TransceiverFactory.CreateNew<NetMQTransceiver>(config);
        var a = tran.Export(true);
    }

}
