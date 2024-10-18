using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MATSys.Automation;
public class TestSummary
{
    public int Ignore { get;private set; }
    public int Total { get; private set; }
    public int Pass { get; private set; }
    public int Fail { get; private set; }
    public int Skip { get; private set; }
    public int Error { get; private set; }

    public void Init()
    {
        Total = Pass = Fail = Skip = Error = Ignore=0;
    }

    public void Update(TestItem item)
    {
        switch (item.Result.Value.Result)
        {
            case TestResultType.Ignore:
                Ignore++;
                Total++;
                break;
            case TestResultType.Skip:
                Skip++;
                Total++;
                break;
            case TestResultType.Pass:
                Pass++;
                Total++;
                break;
            case TestResultType.Fail:
                Fail++;
                Total++;
                break;
            case TestResultType.Error:
                Error++;
                Total++;
                break;
            default:
                break;
        }
    }

    public override string ToString()
    {
        return $"Total:{Total},Pass:{Pass},Fail:{Fail},Skip:{Skip},Error:{Error}";

    }
}
