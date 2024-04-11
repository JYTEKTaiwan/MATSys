using MATSys.Commands;

namespace MATSys;

internal class MATSysContext
{
    public string MethodName { get; }
    public MATSys.Utilities.MethodInvoker Invoker { get; }
    public Type CommandType { get; }

    internal MATSysContext(object target, MethodInfo mi)
    {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
        var attr = mi.GetCustomAttribute<MATSysCommandAttribute>();
#elif NET35||NET462
        var attr = mi.GetCustomAttributes(typeof(MATSysCommandAttribute), true).First() as MATSysCommandAttribute;
#endif
        MethodName = attr.Alias;
        Invoker = MATSys.Utilities.MethodInvoker.Create(target, mi);
        CommandType = GetCommandTypeFromMethodInfo(mi);
    }

    public static MATSysContext Create(object target, MethodInfo mi)
    {
        return new MATSysContext(target, mi);
    }

    private Type GetCommandTypeFromMethodInfo(MethodInfo mi)
    {
        var types = mi.GetParameters().Select(x => x.ParameterType).ToArray();
        switch (types.Length)
        {
            case 0:
                return typeof(Command);
            case 1:
                return typeof(Command<>).MakeGenericType(types);
            case 2:
                return typeof(Command<,>).MakeGenericType(types);
            case 3:
                return typeof(Command<,,>).MakeGenericType(types);
            case 4:
                return typeof(Command<,,,>).MakeGenericType(types);
            case 5:
                return typeof(Command<,,,,>).MakeGenericType(types);
            case 6:
                return typeof(Command<,,,,,>).MakeGenericType(types);
            case 7:
                return typeof(Command<,,,,,,>).MakeGenericType(types);
            default:
                return typeof(Command);
        }

    }

}
