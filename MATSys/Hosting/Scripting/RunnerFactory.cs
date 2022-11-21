using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace MATSys.Hosting.Scripting
{
    internal class RunnerFactory : IRunnerFactory
    {

        /// <summary>
        /// section key for recorder assembly reference paths
        /// </summary>
        private const string sectionKey = "MATSys:References:Runners";
        private readonly static Type defaultType = typeof(ManualRunner);
        private readonly NLog.ILogger _logger;

        /// <summary>
        /// ctor of recorder factory (dynamically load the assemblies and dependencies from the specified path)
        /// </summary>
        /// <param name="config">configuration instance</param>
        public RunnerFactory(IConfiguration config)
        {
            _logger = LogManager.GetCurrentClassLogger();

            // list the Recorder reference paths in the json file
            var plugins = config.GetSection(sectionKey).AsEnumerable(true).Select(x => x.Value).ToArray();
            _logger.Debug($"External references: {JsonSerializer.Serialize(plugins)}");

            //Load plugin assemblies into memoery
            DependencyLoader.LoadPluginAssemblies(plugins);
            _logger.Info($"{plugins.Length} External references is/are loaded");

        }
        /// <summary>
        /// Create new IRecorder instance using specified section of configuration
        /// </summary>
        /// <param name="section">section of configuration object</param>
        /// <returns>IRecorder instance</returns>
        public IRunner CreateRunner(IConfigurationSection section, bool enabled)
        {
            try
            {
                _logger.Trace($"Path: {section.Path}");
                var enableSection = section.GetSection("ScriptMode");
                Type t = defaultType;
                if (enableSection.Exists() && enabled)
                {
                    t = typeof(ScriptRunner);
                    var runnerSection = section.GetSection("Runner");
                    if (runnerSection.Exists())
                    {


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
                }
                _logger.Debug($"{t.Name} is used");

                return CreateRunner(t, section);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Create new IRecorder instance (return DefaultInstance if <paramref name="type"/> is not inherited from IRecorder)
        /// </summary>
        /// <param name="type">type of instance</param>
        /// <param name="section">section of configuration</param>
        /// <returns>IRecorder instance</returns>
        public IRunner CreateRunner(Type type, IConfigurationSection section)
        {
            if (typeof(IRunner).IsAssignableFrom(type))
            {
                var obj = (IRunner)Activator.CreateInstance(type)!;
                return obj;
            }
            else
                return (IRunner)Activator.CreateInstance(defaultType);
        }
        /// <summary>
        /// Create new IRecorder instance by loaded from file (return Default if type is not found) 
        /// </summary>
        /// <param name="assemblyPath">assembly path</param>
        /// <param name="typeString">string of type</param>
        /// <param name="args">parameter instance</param>
        /// <returns>IRecorder instance</returns>

        public bool IsRunnerTypeExist(string typeName)
        {
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            bool result = false;
            if (!string.IsNullOrEmpty(typeName))
            {
                //if key has value, search the type with the default class name. eg. xxx=>xxxRecorder
                foreach (var assem in assems)
                {
                    var dummy = assem.GetTypes().FirstOrDefault(x => x.Name.ToLower() == $"{typeName.ToLower()}");
                    if (dummy == null)
                    {
                        continue;
                    }
                    else
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
        public void Load(IConfigurationSection section)
        {

        }
    }
}
