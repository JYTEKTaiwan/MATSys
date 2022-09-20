using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MATSys.Factories;
using MATSys.Plugins;

namespace UT_MATSys;
public class UT_Export
{

    [Test]
    public void ExportNetMQTransceiver()
    {
        NetMQTransceiverConfiguration config = new NetMQTransceiverConfiguration();
        var tran=TransceiverFactory.CreateNew<NetMQTransceiver>(config);
        var mod = ModuleFactory.CreateNew<UT_ModuleBase.NormalDevice>(null, tran);
        var a = mod.Export(Newtonsoft.Json.Formatting.Indented);
    }

}
