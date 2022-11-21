﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        public static bool LargerThan(this AnalyzingData data,double comparator)
        {
            return JsonSerializer.Deserialize<double>(data.Value) > comparator;
        }

    }
}
