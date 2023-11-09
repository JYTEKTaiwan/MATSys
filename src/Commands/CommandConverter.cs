using MATSys.Commands;


/// <summary>
/// Tool used to convert any instance marked with <see cref="MATSysCommandContractAttribute"/> and <see cref="MATSysCommandOrderAttribute"/> into ICommand instance
/// </summary>
public sealed class CommandConverter
{
    private static IEnumerable<MethodInfo> m_infos => typeof(CommandBase).GetMethods().Where(x => x.Name == "Create");
    private static void CheckContract(Type t)
    {
        if (t.GetCustomAttribute<MATSysCommandContractAttribute>() == null)
        {
            throw new InvalidOperationException("MATSysCommandContract attribute is not presented");
        }
    }


    /// <summary>
    /// Convert the object into ICommand instance
    /// </summary>
    /// <param name="input">input object</param>
    /// <returns>ICommand instance</returns>
    public static ICommand Convert(object input)
    {
        var t = input.GetType();
        //check if input is MATSysCommandContractAttribute
        CheckContract(t);

        var name_attr = t.GetCustomAttribute<MATSysCommandContractAttribute>()!;
        if (name_attr == null) throw new NoNullAllowedException("MATSysCommandContractAttribute is not exited in the input");
        var name = name_attr.MethodName;

        var props = t.GetProperties()
        .Where(x => x.GetCustomAttributes<MATSysCommandOrderAttribute>().Count() != 0)
        .OrderBy(x => x.GetCustomAttribute<MATSysCommandOrderAttribute>()!.Order).ToList();

        object[] p_name = new object[] { name };
        var mi = m_infos.First(x => x.GetGenericArguments().Count() == props.Count());
        if (props.Count == 0) return (mi.Invoke(null, p_name) as ICommand)!;
        else
        {
            Type[] types = props.Select(x => x.PropertyType).ToArray();
            IEnumerable<object> p_rest = props.Select(x => x.GetValue(input)!);
            var param = p_name.Concat(p_rest).ToArray();
            return (mi.MakeGenericMethod(types).Invoke(null, param) as ICommand)!;
        }

    }
}
