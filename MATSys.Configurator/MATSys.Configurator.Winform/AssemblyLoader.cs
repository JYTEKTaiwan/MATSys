using System.Collections;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MATSys.Configurator.Core
{
    public class AssemblyLoader
    {
        private readonly string _binFolder;
        public AssemblyLoader(string binFolderPath)
        {
            _binFolder = binFolderPath;
            var paths = Directory.GetFiles(binFolderPath, "*.dll").Concat(
                Directory.GetFiles(binFolderPath + @"\modules", "*.dll"));
            foreach (var item in paths)
            {
                try
                {
                    Assembly.LoadFrom(item);
                }
                catch (Exception) { }

            }
        }
        public IEnumerable<MATSysInformation> ListAllModules()
        {
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            var assem = assems.FirstOrDefault(x => x.GetName().Name == "MATSys");
            Type t = assem.GetType("MATSys.IModule");
            //search executing assembly
            foreach (var a in assems)
            {
                foreach (var item in a.GetTypes())
                {
                    if (t.IsAssignableFrom(item) && !item.IsAbstract && !item.IsInterface)
                    {
                        yield return new MATSysInformation()
                        {
                            Alias = item.Name,
                            Type = item.Name,
                            Category = "Module",
                            IsExternal = a.Location.Contains(@"modules\"),
                            AssemblyPath = Path.GetRelativePath(_binFolder, a.Location),
                        };

                    }

                }
            }

        }
        public IEnumerable<MATSysInformation> ListAllPlugins()
        {
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            var assem = assems.FirstOrDefault(x => x.GetName().Name == "MATSys");
            Type t = assem.GetType("MATSys.IPlugin");

            //search executing assembly
            foreach (var a in assems)
            {
                foreach (var item in a.GetTypes())
                {
                    if (t.IsAssignableFrom(item) && !item.IsAbstract && !item.IsInterface)
                    {

                        yield return new MATSysInformation()
                        {
                            Alias = Parse(item.Name).name,
                            Category = Parse(item.Name).type,
                            Type = item.Name,
                            Setting = GetConfiguration(item),
                            IsExternal = a.Location.Contains(@"modules\"),
                            AssemblyPath = Path.GetRelativePath(_binFolder, a.Location),
                        };
                    }
                }
            }
        }
        private (string name, string type) Parse(string input)
        {

            var name = "";
            var type = "";
            var option = StringSplitOptions.None;
            if (input.Contains("Transceiver"))
            {
                name = input.Split(new string[] { "Transceiver" }, option)[0];
                type = "Transceiver";
            }
            else if (input.Contains("Notifier"))
            {
                name = input.Split(new string[] { "Notifier" }, option)[0];
                type = "Notifier";
            }
            else if (input.Contains("Recorder"))
            {
                name = input.Split(new string[] { "Recorder" }, option)[0];
                type = "Recorder";
            }
            else
            {
                name = "";
                type = "";
            }
            name = name.ToLower();
            name = name == "empty" ? "" : name;
            return (name, type);
        }
        private object? GetConfiguration(Type type)
        {
            var name = type.Name;
            var searchKey = name + "Configuration";
            var assem = type.Assembly;
            var value = assem.GetTypes().FirstOrDefault(x => x.Name == searchKey);
            if (value != null)
            {
                return Activator.CreateInstance(value);

            }
            return null;
        }

        public Type? GetModuleType(string typeName)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
            return types.FirstOrDefault(x => x.Name == typeName);
        }
        public IEnumerable<string> ShowSupportedCommands(string typeName)
        {
            var t = GetModuleType(typeName);
            if (t != null)
            {
                return ShowSupportedCommands(t);
            }
            else
            {
                return new string[0];
            }

        }
        public IEnumerable<string> ShowSupportedCommands(Type t)
        {
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            var assem = assems.FirstOrDefault(x => x.GetName().Name == "MATSys");
            Type t_module = assem.GetType("MATSys.IModule");
            Type t_cmdAtt = assem.GetType("MATSys.Commands.MATSysCommandAttribute");
            if (t_module.IsAssignableFrom(t))
            {
                var methods = t.GetMethods().Where(x => x.GetCustomAttribute(t_cmdAtt) != null);
                foreach (var mi in methods)
                {
                    var cmd = mi.GetCustomAttribute(t_cmdAtt);

                    if (cmd != null)
                    {
                        cmd.GetType().GetMethod("ConfigureCommandType").Invoke(cmd, new object[] { mi });
                        yield return cmd.GetType().GetMethod("GetTemplateString").Invoke(cmd, null) as string;

                    }
                    else
                    {
                        continue;
                    }

                }
            }


        }

    }

    public class ExportUtility
    {
        public static void SaveToFile( JsonNode node, string folderPath="")
        {
            var jsonOpt = new JsonSerializerOptions() { WriteIndented = true };

            var p = Path.GetFullPath(Path.Combine(folderPath, "appsettings.json"));
            if (File.Exists(p))
            {
                var bakPath = Path.Combine(Path.GetDirectoryName(p), "appsettings_backup.json");
                if (File.Exists(bakPath))
                {
                    File.Delete(bakPath);
                }
                File.Move(p, bakPath);
            }
            string jsonString = node.ToJsonString(jsonOpt);
            File.WriteAllText(p, jsonString);
        }
        public static JsonNode ExportToJsonNode(IEnumerable<ExportingDataType> list)
        {
            var jsonOpt = new JsonSerializerOptions() { WriteIndented = true };
            JsonObject jRoot = new JsonObject();

            JsonObject jMatsys = new JsonObject();

            var mods = new JsonArray();
            foreach (var item in list)
            {
                mods.Add(item.ToJsonNode());
            }
            jMatsys.Add("Modules", mods);

            JsonObject jRef = new JsonObject();
            var refs = list.Select(x => x.Module).Where(x => x.IsExternal).Select(x => x.AssemblyPath).Distinct().ToArray();
            jRef.Add("Modules", JsonSerializer.SerializeToNode(refs,jsonOpt));
            refs = list.Select(x => x.Transceiver).Where(x => x.IsExternal).Select(x => x.AssemblyPath).Distinct().ToArray();
            jRef.Add("Transceivers", JsonSerializer.SerializeToNode(refs, jsonOpt));
            refs = list.Select(x => x.Notifier).Where(x => x.IsExternal).Select(x => x.AssemblyPath).Distinct().ToArray();
            jRef.Add("Notifiers", JsonSerializer.SerializeToNode(refs, jsonOpt));
            refs = list.Select(x => x.Recorder).Where(x => x.IsExternal).Select(x => x.AssemblyPath).Distinct().ToArray();
            jRef.Add("Recorders", JsonSerializer.SerializeToNode(refs, jsonOpt));

            jMatsys.Add("References", jRef);

            jRoot.Add("MATSys", jMatsys);

            return jRoot;
        }

    }

    public class ExportingDataType
    {
        public string Alias { get; set; }
        public MATSysInformation Module { get; set; }
        public MATSysInformation Transceiver { get; set; }
        public MATSysInformation Notifier { get; set; }
        public MATSysInformation Recorder { get; set; }
        public ExportingDataType(string alias, MATSysInformation module, MATSysInformation transceiver, MATSysInformation notifier, MATSysInformation recorder)
        {
            Alias = alias;
            Module = module;
            Transceiver = transceiver;
            Notifier = notifier;
            Recorder = recorder;
        }
        public string[] ListAssembliesCollection()
        {
            List<MATSysInformation> list = new List<MATSysInformation>() { Module, Transceiver, Notifier, Recorder };
            return list.Where(x => x.IsExternal).Select(x => x.AssemblyPath).ToArray();
        }
        public JsonNode ToJsonNode()
        {

            var jsonOpt = new JsonSerializerOptions() { WriteIndented = true };
            var node_tran = JsonSerializer.SerializeToNode(Transceiver.Setting, jsonOpt);
            var node_not = JsonSerializer.SerializeToNode(Notifier.Setting, jsonOpt);
            var node_rec = JsonSerializer.SerializeToNode(Recorder.Setting, jsonOpt);
            JsonObject jObj = new JsonObject();
            jObj.Add("Alias", Alias);
            jObj.Add("Type", Module.Type);
            if (node_tran != null)
            {
                jObj.Add("Transceiver", node_tran);
            }
            if (node_not != null)
            {
                jObj.Add("Notifier", node_not);
            }
            if (node_rec != null)
            {
                jObj.Add("Recorder", node_rec);
            }

            return jObj;
        }
    }
    public class MATSysInformation
    {
        public bool IsExternal { get; set; }
        public string? Alias { get; set; }
        public string? Category { get; set; }
        public string? AssemblyPath { get; set; }
        public string? Type { get; set; }
        public object? Setting { get; set; }
        public override string ToString()
        {
            return Type;
        }
    }
    public class PropertyPair
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public PropertyPair(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }


}