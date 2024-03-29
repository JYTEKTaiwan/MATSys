﻿using Microsoft.Extensions.Configuration;

namespace MATSys.Hosting
{
    /// <summary>
    /// Dynamically load the assemblies for AnalyzingData extended methods
    /// </summary>
    internal class AnalyzerLoader
    {
        private const string sectionKey = "MATSys:References:Analyzers";

        /// <summary>
        /// Search for the paths in the section and dynamically load to the AppDomain
        /// </summary>
        /// <param name="config"></param>
        public AnalyzerLoader(IConfiguration config)
        {
            // list the Analyzer reference paths in the json file
            var plugins = config.GetSection(sectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();

            //Load analyzer assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);

        }

    }
}
