using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MATSys.Hosting
{

    public struct AnalyzingData
    {
        public string Value { get; }
        public AnalyzingData(string data)
        {
            Value = data;
        }
        public static AnalyzingData Create(string input)
        {
            return new AnalyzingData(input);
        }
    }

    public static class Analyzer
    {
        public static bool LargerThan(this AnalyzingData data,double result)
        {
            return JsonSerializer.Deserialize<double>(data.Value) > result;
        }
    }
}
