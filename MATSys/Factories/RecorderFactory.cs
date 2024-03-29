﻿using MATSys.Plugins;
using Microsoft.Extensions.Configuration;
using NLog;
using System.Reflection;
using System.Text.Json;

namespace MATSys.Factories
{
    /// <summary>
    /// Factory used to create recorder
    /// </summary>
    public sealed class RecorderFactory : IRecorderFactory
    {

        /// <summary>
        /// searching prefix string
        /// </summary>
        private const string prefix = "Recorder";
        /// <summary>
        /// section key for recorder assembly reference paths
        /// </summary>
        private const string sectionKey = "MATSys:References:Recorders";
        private readonly static Type DefaultType = typeof(EmptyRecorder);
        private static Lazy<IRecorder> _default = new Lazy<IRecorder>(() => new EmptyRecorder());
        private static IRecorder DefaultInstance => _default.Value;
        private readonly NLog.ILogger _logger;

        /// <summary>
        /// ctor of recorder factory (dynamically load the assemblies and dependencies from the specified path)
        /// </summary>
        /// <param name="config">configuration instance</param>
        public RecorderFactory(IConfiguration config)
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
        public IRecorder CreateRecorder(IConfigurationSection section)
        {
            try
            {
                _logger.Trace($"Path: {section.Path}");

                Type t = DefaultType;
                //check if section in the json configuration exits
                if (section.Exists())
                {
                    string type = section.GetValue<string>("Type");
                    if (!string.IsNullOrEmpty(type))
                    {
                        if (type.Contains("."))
                        {
                            //look up in the GAC
                            var dummy = Type.GetType(Assembly.CreateQualifiedName(type, type));
                            if (dummy != null)
                            {
                                t = dummy;
                            }
                        }
                        else
                        {
                            //look up in the loaded assemblies
                            var assems = AppDomain.CurrentDomain.GetAssemblies();
                            foreach (var assem in assems)
                            {
                                try
                                {

                                    var dummy = assem.GetTypes().FirstOrDefault(x => x.Name.ToLower() == $"{type}{prefix}".ToLower());
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
                                catch (Exception ex)
                                {

                                    throw;
                                }

                            }

                        }
                    }
                }
                _logger.Debug($"{t.Name} is used");

                return CreateRecorder(t, section);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Create new IRecorder instance (return DefaultInstance if <paramref name="defaultType"/> is not inherited from IRecorder)
        /// </summary>
        /// <param name="defaultType">type of instance</param>
        /// <param name="section">section of configuration</param>
        /// <returns>IRecorder instance</returns>
        private IRecorder CreateRecorder(Type defaultType, IConfigurationSection section)
        {
            if (typeof(IRecorder).IsAssignableFrom(defaultType))
            {
                var obj = (IRecorder)Activator.CreateInstance(defaultType)!;
                obj.Load(section);
                return obj;
            }
            else
                return DefaultInstance;
        }
        /// <summary>
        /// Create new IRecorder instance by loaded from file (return Default if type is not found) 
        /// </summary>
        /// <param name="assemblyPath">assembly path</param>
        /// <param name="typeString">string of type</param>
        /// <param name="args">parameter instance</param>
        /// <returns>IRecorder instance</returns>
        public static IRecorder CreateNew(string assemblyPath, string typeString, object args)
        {
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = Assembly.LoadFile(Path.GetFullPath(assemblyPath));
            if (assems.FirstOrDefault(x => x.FullName == assembly.FullName) != null)
            {
                //exists
            }
            else
            {
                DependencyLoader.LoadPluginAssemblies(new string[] { assemblyPath });
            }
            try
            {

                assems = AppDomain.CurrentDomain.GetAssemblies();

                Type t = DefaultType;
                //check if section in the json configuration exits

                if (!string.IsNullOrEmpty(typeString))
                {
                    //if key has value, search the type with the default class name. eg. xxx=>xxxRecorder
                    foreach (var assem in assems)
                    {
                        var dummy = assem.GetTypes().FirstOrDefault(x => x.Name.ToLower() == $"{typeString}{prefix}".ToLower());
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
                var obj = (IRecorder)Activator.CreateInstance(t)!;
                obj.Load(args);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Create <typeparamref name="T"/> instance statically
        /// </summary>
        /// <param name="args">parameter object</param>
        /// <typeparam name="T">type inherited from IRecorder</typeparam>
        /// <returns><typeparamref name="T"/> instance</returns>
        public static T CreateNew<T>(object args) where T : IRecorder
        {
            var obj = (T)Activator.CreateInstance(typeof(T))!;
            obj?.Load(args);
            return obj!;
        }
        /// <summary>
        /// Create IRecorder instance statically (return Default instance if <paramref name="T"/> is not inherited from IRecorder)
        /// </summary>
        /// <param name="t">type</param>
        /// <param name="args">parameter object</param>
        /// <returns>IRecorder instance</returns>
        public static IRecorder CreateNew(Type t, object args)
        {
            if (typeof(IRecorder).IsAssignableFrom(t))
            {
                var obj = Activator.CreateInstance(t) as IRecorder;
                obj?.Load(args);
                return obj!;
            }
            else

                return DefaultInstance;
        }

    }
}