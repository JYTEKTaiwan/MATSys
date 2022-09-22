
using MATSys;
using MATSys.Commands;
using MATSys.Factories;
using MATSys.Plugins;
using System.Reflection;
using System.Security.Policy;

string subFolder = @".\modules";
if (Directory.Exists(subFolder))
{
    DependencyLoader.LoadPluginAssemblies(Directory.GetFiles(subFolder));
}


foreach (var item in MATSysContext.SearchPlugins())
{
    Console.WriteLine($"{item.Category}\t{item.Type}\t{item.AssemblyPath}");
}
foreach (var item in MATSysContext.SearchModules())
{
    Console.WriteLine($"{item.Category}\t{item.Type}\t{item.AssemblyPath}");
}
Console.ReadKey();



public class Test : ModuleBase
{
    public Test(object? configuration, ITransceiver? transceiver, INotifier? notifier, IRecorder? recorder, string aliasName = "") : base(configuration, transceiver, notifier, recorder, aliasName)
    {
    }

    public override void Load(Microsoft.Extensions.Configuration.IConfigurationSection section)
    {
        throw new NotImplementedException();
    }

    public override void Load(object configuration)
    {
        throw new NotImplementedException();
    }

    [MATSysCommand]
    public void Tesrrt()
    {

    }
}

public class MATSysContext
{
    public string ModuleAlias { get; set; }
    public string Category { get; set; }
    public bool IsExternalAssembly { get; set; }
    public string AssemblyPath { get; set; }
    public string Type { get; set; }

    public static IEnumerable<MATSysContext> SearchPlugins()
    {
        List<MATSysContext> list = new List<MATSysContext>();
        Type t = typeof(IPlugin);

        //search executing assembly
        foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var item in assem.GetTypes())
            {
                if (t.IsAssignableFrom(item) && !item.IsAbstract && !item.IsInterface)
                {
                    list.Add(new MATSysContext()
                    {
                        Type = Parse(item.Name).name,
                        Category = Parse(item.Name).type,
                        ModuleAlias = item.Name,
                        IsExternalAssembly = false,
                        AssemblyPath = assem.Location
                    }); ; ;
                }
            }
        }
        return list;

    }

    public static IEnumerable<MATSysContext> SearchModules()
    {
        List<MATSysContext> list = new List<MATSysContext>();
        Type t = typeof(IModule); 
        

        //search executing assembly
        foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var item in assem.GetTypes())
            {
                if (t.IsAssignableFrom(item) && !item.IsAbstract && !item.IsInterface)
                {
                    list.Add(new MATSysContext()
                    {
                        ModuleAlias = item.Name,
                        Type = item.Name,
                        Category = "Modules",
                        IsExternalAssembly = false,
                        AssemblyPath = assem.Location
                    });

                }

            }
        }

        return list;
    }

    private static (string name, string type) Parse(string input)
    {

        var name = "";
        var type = "";
        if (input.Contains("Transceiver"))
        {
            name = input.Split("Transceiver")[0];
            type = "Transceiver";
        }
        else if (input.Contains("Notifier"))
        {
            name = input.Split("Notifier")[0];
            type = "Notifier";
        }
        else if (input.Contains("Recorder"))
        {
            name = input.Split("Recorder")[0];
            type = "Recorder";
        }
        else
        {
            name = "";
            type = "";
        }
        name=name.ToLower();
        name = name == "empty" ? "" : name;
        return (name, type);
            


    }

}