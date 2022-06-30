// See https://aka.ms/new-console-template for more information
using MATSys;
using MATSys.Commands;
using System.CommandLine;
using System.Reflection;
using Command = System.CommandLine.Command;

string _moduleFolder = Path.GetFullPath(@".\modules");
var delayOption = new Option<int>
(name: "--delay",
description: "An option whose argument is parsed as an int.",
getDefaultValue: () => 42);
var messageOption = new Option<string>
    (name: "--message",
    description: "An option whose argument is parsed as a string.");

var rootCommand = new RootCommand("Sample command-line app");
rootCommand.SetHandler(() => { Console.WriteLine("HELLO"); });
rootCommand.AddOption(delayOption);

var devCommand = new Command("dev", "Show device information");
devCommand.SetHandler(() => { Console.WriteLine(_moduleFolder); });

var listDeviceCommand = new Command("list", "List all supported devices in the directory");
var directoryOption = new Option<string>("-d", "Directory path where devices located");
listDeviceCommand.AddOption(directoryOption);
listDeviceCommand.SetHandler((dict) => { ListDevices(dict); }, directoryOption);
devCommand.AddCommand(listDeviceCommand);

var infoCommand = new Command("info", "Display the information of device implements IDevice");
var nameOption = new Option<string>("-name", "Name of device");
infoCommand.AddOption(directoryOption);
infoCommand.AddOption(nameOption);
infoCommand.SetHandler((dict, name) => { DisplayDevice(dict, name); }, directoryOption, nameOption);
devCommand.Add(infoCommand);

var modCommand = new Command("mod", "Show module information");
modCommand.SetHandler(() => { Console.WriteLine(_moduleFolder); });

var listModuleCommand = new Command("list", "List all supported modules in the directory");
listModuleCommand.AddOption(directoryOption);
listModuleCommand.SetHandler((dict) => { ListModules(dict); }, directoryOption);
modCommand.AddCommand(listModuleCommand);

rootCommand.Add(devCommand);
rootCommand.Add(modCommand);
await rootCommand.InvokeAsync(args);

static void ListDevices(string path)
{
    var folder = TrimPathString(path);

    foreach (var item in Directory.GetFiles(Path.GetFullPath(folder), "*.dll"))
    {
        var types = Assembly.LoadFile(item).GetTypes().Where
            (x => !x.IsInterface && !x.IsAbstract && x.GetInterface(typeof(IDevice).FullName!) != null);
        foreach (var t in types)
        {
            Console.WriteLine($"{t.Name.PadRight(50)}  {item}");
        }
    }
}
static void ListModules(string path)
{
    var folder = TrimPathString(path);

    foreach (var item in Directory.GetFiles(Path.GetFullPath(folder), "*.dll"))
    {
        var types = Assembly.LoadFile(item).GetTypes().Where
            (x => !x.IsInterface && !x.IsAbstract && x.GetInterface(typeof(IModule).FullName!) != null);
        foreach (var t in types)
        {
            Console.WriteLine($"{t.Name.PadRight(50)}  {item}");
        }
    }
}

static void DisplayDevice(string path, string name)
{
    var folder = TrimPathString(path);
    foreach (var item in Directory.GetFiles(Path.GetFullPath(folder), "*.dll"))
    {
        var type = Assembly.LoadFile(item).GetTypes().FirstOrDefault
            (x => x.Name == name && x.GetInterface(typeof(IDevice).FullName!) != null);
        if (type != null)
        {
            var methodlist = type.GetMethods().Where(x =>
            {
                return x.GetCustomAttributes<PrototypeAttribute>(false).Count() > 0;
            }).ToArray();

            foreach (var method in methodlist)
            {
                var att = method.GetCustomAttribute<PrototypeAttribute>();
                Console.WriteLine($"{method.ToString().PadRight(50)} \t {att.GetJsonString()}");
            }
            return;
        }
    }
}

static string TrimPathString(string path)
{
    var temp = String.IsNullOrEmpty(path) ? @".\" : path;
    var dict = new Dictionary<string, string>(){
        {"\a", "\\a"},
        {"\b", "\\b"},
        {"\t", "\\t"},
        {"\n", "\\n"},
        {"\v", "\\v"},
        {"\f", "\\f"},
        {"\r", "\\r"}
    };
    foreach (var item in dict.Select(x => x.Key))
    {
        temp = temp.Replace(item, dict[item]);
    }
    return temp;
}