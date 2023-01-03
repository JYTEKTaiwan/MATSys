using System.Text.Json;

namespace MATSys.Hosting
{

    /// <summary>
    /// Data struct used to be processed (string format)
    /// </summary>
    public struct AnalyzingData
    {
        /// <summary>
        /// Input value (json formatted string)
        /// </summary>
        public string Value { get; }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="data">json formatted string</param>
        public AnalyzingData(string data)
        {
            Value = data;
        }
        /// <summary>
        /// Create new instance of AnalyzingData
        /// </summary>
        /// <param name="input">json formatted string</param>
        /// <returns>instance of AnalyzingData</returns>
        public static AnalyzingData Create(string input)
        {
            return new AnalyzingData(input);
        }
    }

    /// <summary>
    /// Class that is used to analyze the AnalyzingData object
    /// </summary>
    public static class Analyzer
    {
        /// <summary>
        /// Larger Than 
        /// </summary>
        /// <param name="data">intput data</param>
        /// <param name="comparator">condition in double format</param>
        /// <returns>pass or fail</returns>
        public static bool LargerThan(this AnalyzingData data, double comparator)
        {
            return JsonSerializer.Deserialize<double>(data.Value) > comparator;
        }
        public static bool SmallerThan(this AnalyzingData data, double comparator)
        {
            return JsonSerializer.Deserialize<double>(data.Value) < comparator;
        }
        public static bool Between(this AnalyzingData data, double low, double high)
        {
            var v = JsonSerializer.Deserialize<double>(data.Value);
            return v > low && v < high;
        }
        public static bool EqualsTo(this AnalyzingData data, double comparator)
        {
            return JsonSerializer.Deserialize<double>(data.Value) == comparator;
        }
        public static bool IsTrue(this AnalyzingData data)
        {
            var v = JsonSerializer.Deserialize<bool>(data.Value);
            return v;
        }

    }
}
