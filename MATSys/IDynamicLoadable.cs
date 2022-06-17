namespace MATSys
{
    public interface IDynamicLoadable
    {
        delegate System.Reflection.Assembly? AssemblyResolve(object? sender, ResolveEventArgs args);

        AssemblyResolve ResolveAction { get; }

    }
}