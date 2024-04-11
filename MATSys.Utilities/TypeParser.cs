using System.Reflection;

namespace MATSys.Utilities;

public static class TypeParser
{
    /// <summary>
    /// Search the Type from entry, executing, calling  and external assemblies in sequence. 
    /// </summary>
    /// <param name="type">full name of the type</param>
    /// <param name="extAssemPath">external assembly path</param>
    /// <returns>Type instance, will be null if <paramref name="type"/> is empty or null, or if Type cannot be found</returns>
    public static Type? SearchType(string type, string extAssemPath)
    {
        // return EmptyRecorder if type is empty or null
        if (string.IsNullOrEmpty(type)) return null;

        try
        {
            

            var typeName = Assembly.CreateQualifiedName(type, type).Split(',')[0];
            var t = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).FirstOrDefault(x => x.FullName == typeName);
            if (t != null) return t;

            t = DynamicLibraryLoader.LoadPluginAssemblies(extAssemPath).First().GetType(type);
            return t;

        }
        catch (System.Exception ex)
        {
            throw new InvalidOperationException($"Failed to find type named {type}. Message:{ex.Message}");
        }
    }

}
