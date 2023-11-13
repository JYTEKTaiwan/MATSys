using System.Diagnostics.CodeAnalysis;
using MATSys.Commands;

namespace MATSys;

internal class MATSysContext
{
    public string MethodName { get; }
    public MethodInvoker Invoker { get; }
    public Type CommandType { get; }

    internal MATSysContext(object target, MethodInfo mi)
    {
        var cmd = mi.GetCustomAttribute<MATSysCommandAttribute>()!;
        MethodName = cmd.Alias;
        Invoker = MethodInvoker.Create(target,mi);


        var types = mi.GetParameters().Select(x => x.ParameterType).ToArray();
        Type t = GetGenericCommandType(types.Length);
        if (t.IsGenericType)
        {
            CommandType = t.MakeGenericType(types);
        }
        else
        {
            CommandType = t;
        }


    }

    public static MATSysContext Create(object target, MethodInfo mi)
    {
        return new MATSysContext(target, mi);
    }

    private Type GetGenericCommandType(int count)
    {
        switch (count)
        {
            case 0:
                return typeof(Command);
            case 1:
                return typeof(Command<>);
            case 2:
                return typeof(Command<,>);
            case 3:
                return typeof(Command<,,>);
            case 4:
                return typeof(Command<,,,>);
            case 5:
                return typeof(Command<,,,,>);
            case 6:
                return typeof(Command<,,,,,>);
            case 7:
                return typeof(Command<,,,,,,>);
            default:
                return typeof(Command);
        }

    }

}
