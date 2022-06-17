using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace MATSys.Plugins;

public abstract class DartaRecorderBase : IDataRecorder
{
    public abstract string Name { get; }

    public IDataRecorder.AssemblyResolve ResolveAction =>AssemblyResolve;

    public virtual Assembly? AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        string s1 = args.Name.Remove(args.Name.IndexOf(',')) + ".dll";
        string s2 = Path.Combine(DependencyLoader.Instance.LibrariesFolder, args.Name.Remove(args.Name.IndexOf(',')) + ".dll");
        if (File.Exists(s1))
        {
            return Assembly.LoadFile(Path.GetFullPath(s1));
        }
        else if (File.Exists(s2))
        {
            return Assembly.LoadFile(Path.GetFullPath(s2));
        }
        else
        {
            throw new FileLoadException($"Dependent assembly not found : {args.Name}");
        }
    }

    public abstract void Load(IConfigurationSection section);

    public abstract Task StartServiceAsync(CancellationToken token);

    public abstract void StopService();

    public abstract void Write(object data);

    public abstract Task WriteAsync(object data);
}