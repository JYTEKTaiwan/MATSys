using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MATSys.Hosting
{

    public struct AnalyzeData
    {
        public string Value { get; }
        public AnalyzeData(string data)
        {
            Value = data;
        }
        public static AnalyzeData Create(string input)
        {
            return new AnalyzeData(input);
        }
    }

    public static class Analyzer
    {
        public static bool LargerThan(this AnalyzeData data,double result)
        {
            return JsonSerializer.Deserialize<double>(data.Value) > result;
        }
    }
}
