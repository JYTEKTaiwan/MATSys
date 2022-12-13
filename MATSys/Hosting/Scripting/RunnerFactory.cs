using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace MATSys.Hosting.Scripting
{

    /// <summary>
    /// Runner Factory
    /// </summary>
    internal class RunnerFactory : IRunnerFactory
    {

        private const string referencesSectionKey = "MATSys:References:Runners";
        private const string runnerSectionKey = "MATSys:Runner";
        private readonly static Type defaultType = typeof(ManualRunner);
        private readonly NLog.ILogger _logger;
        private readonly IConfiguration _config;

        /// <summary>
        /// ctor of runner factory (dynamically load the assemblies and dependencies from the specified path)
        /// </summary>
        /// <param name="config">configuration instance</param>
        public RunnerFactory(IConfiguration config)
        {
            _config= config;
            _logger = LogManager.GetCurrentClassLogger();

            // list the runner reference paths in the json file
            var plugins = config.GetSection(referencesSectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();
            _logger.Debug($"External references: {JsonSerializer.Serialize(plugins)}");

            //Load runner assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);
            _logger.Info($"{plugins.Length} External references is/are loaded");

        }
        /// <summary>
        /// Create Runner instance
        /// </summary>
        /// <param name="section">Configuration section in json file</param>
        /// <param name="enabled">if script mode is enable or not</param>
        /// <returns><see cref="IRunner"/> instance</returns>
        public IRunner CreateRunner()
        {
            try
            {
                var runnerSection = _config.GetSection(runnerSectionKey);
                _logger.Trace($"Path: {runnerSection.Path}");
                Type t = defaultType;
                if (runnerSection.Exists())
                {
                    t = typeof(ScriptRunner);
                    var assems = AppDomain.CurrentDomain.GetAssemblies();
                    string type = runnerSection.GetValue<string>("Type");
                    if (!string.IsNullOrEmpty(type))
                    {
                        //if key has value, search the type with the default class name. eg. xxx=>xxxRecorder
                        foreach (var assem in assems)
                        {
                            var dummy = assem.GetTypes().FirstOrDefault(x => x.Name.ToLower() == $"{type.ToLower()}");
                            if (dummy == null)
                            {
                                continue;
                            }
                            else
                            {
                                t = dummy;
                                break;
                            }
                        }
                    }
                }
                _logger.Debug($"{t.Name} is used");

                return CreateNew(t);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Create Runner instance
        /// </summary>
        /// <param name="type">Type of the runner</param>
        /// <param name="section">Configuration section in json file</param>
        /// <returns><see cref="IRunner"/> instance</returns>
        private IRunner CreateNew(Type type)
        {
            IRunner obj= null;
            if (typeof(IRunner).IsAssignableFrom(type))
            {
                obj = (IRunner)Activator.CreateInstance(type)!;
                
            }
            else
                obj= (IRunner)Activator.CreateInstance(defaultType);

            var settingPath = ((_config as IConfigurationRoot).Providers.FirstOrDefault(x => x is JsonConfigurationProvider) as JsonConfigurationProvider).Source.Path;
            var node = JsonNode.Parse(File.ReadAllText(settingPath))["MATSys"]["Runner"];
            obj.Load(node);
            return obj;
        }
    }
}
