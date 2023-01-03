using MATSys.Factories;
using MATSys.Plugins;

namespace UT_MATSys;
public class UT_Export
{

    [Test]
    public void ExportNetMQTransceiver()
    {
        NetMQTransceiverConfiguration config = new NetMQTransceiverConfiguration();
        var tran = TransceiverFactory.CreateNew<NetMQTransceiver>(config);
        var mod = ModuleFactory.CreateNew<NormalDevice>(null, tran);
        var a = mod.Export(true);
    }

}
