using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MATSys.Hosting
{
    public partial class Analyzer
    {
        public static bool LargerThan(double result, string input)
        {
            return JsonSerializer.Deserialize<double>(input) > result;
        }
    }
}
