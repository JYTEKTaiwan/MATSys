using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MATSys.Hosting
{
    internal class AnalyzerLoader
    {
        private const string sectionKey = "MATSys:References:Analyzers";
        
        public AnalyzerLoader(IConfiguration config)
        {
            // list the Analyzer reference paths in the json file
            var plugins = config.GetSection(sectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();        

            //Load analyzer assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);        

        }

    }
}
