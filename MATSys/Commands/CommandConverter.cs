namespace MATSys.Commands;
/// <summary>
/// Tool used to convert any instance marked with <see cref="MATSysCommandContractAttribute"/> and <see cref="MATSysCommandOrderAttribute"/> into ICommand instance
/// </summary>
public sealed class CommandConverter

{
#if NET6_0_OR_GREATER || NETSTANDARD2_0
    private static Lazy<IEnumerable<MethodInfo>> m_infos => new Lazy<IEnumerable<MethodInfo>>(() => typeof(CommandBase).GetMethods().Where(x => x.Name == "Create"));

#elif NET35
    private static IEnumerable<MethodInfo> m_infos = typeof(CommandBase).GetMethods().Where(x => x.Name == "Create");

#endif
    //All generic method from CommandBase.Create

    private static MATSysCommandContractAttribute GetContractAttribute(Type t)
    {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
        var attr = t.GetCustomAttribute<MATSysCommandContractAttribute>();
#elif NET35
        var attr = t.GetCustomAttributes(typeof(MATSysCommandContractAttribute), true).First() as MATSysCommandContractAttribute;
#endif
        if (attr == null)
        {
            throw new InvalidOperationException("MATSysCommandContract attribute is not presented");
        }
        else
        {
            return attr;
        }
    }
    private static bool CheckIfRecordType(Type t)
    {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
        return ((TypeInfo)t).DeclaredProperties.Any(x => x.Name == "EqualityContract");
#elif NET35
        //TypeInfo is not supported for net35, return alse directly 
        return false;
#endif
    }
    private static Func<PropertyInfo, bool> ContainMATSysCommandOrderAttribute =>
    pi =>
{
#if NET6_0_OR_GREATER || NETSTANDARD2_0
        return pi.GetCustomAttributes<MATSysCommandOrderAttribute>().Any();
#elif NET35
    return pi.GetCustomAttributes(typeof(MATSysCommandOrderAttribute), true).Any();
#endif
};
    private static Func<PropertyInfo, int> GetMATSysCommandOrderAttributeOrder =>
        pi =>
        {
#if NET6_0_OR_GREATER || NETSTANDARD2_0
        return pi.GetCustomAttribute<MATSysCommandOrderAttribute>().Order;
#elif NET35
            var attr = pi.GetCustomAttributes(typeof(MATSysCommandOrderAttribute), true).First();
            return (attr as MATSysCommandOrderAttribute).Order;
#endif
        };

    /// <summary>
    /// Convert the object into ICommand instance
    /// </summary>
    /// <param name="input">input object</param>
    /// <returns>ICommand instance</returns>
    public static ICommand Convert(object input)
    {
        try
        {
            var t = input.GetType();
            //check if input is MATSysCommandContractAttribute          
            var name_attr = GetContractAttribute(t);
            var name = name_attr.MethodName;
            var props = t.GetProperties();
            var hasOrderAttr = props.Any(ContainMATSysCommandOrderAttribute);
            if (hasOrderAttr | !CheckIfRecordType(t))
            {
                props = props
                    .Where(ContainMATSysCommandOrderAttribute)
                    .OrderBy(GetMATSysCommandOrderAttributeOrder).ToArray();
            }

#if NET6_0_OR_GREATER || NETSTANDARD2_0
            var mi = m_infos.Value.First(x => x.GetGenericArguments().Count() == props.Count());

#elif NET35
            var mi = m_infos.First(x => x.GetGenericArguments().Count() == props.Count());
#endif
            //Get the method that matches the count of properties

            //first argument will be the name of method
            object[] p_name = new object[] { name };

            //return Command instance if no arguments is required
            if (props.Count() == 0) return (mi.Invoke(null, p_name) as ICommand)!;
            else
            {
                // if arguments count is larger than 0, assign the generic method and dynamically create the Command
                Type[] types = props.Select(x => x.PropertyType).ToArray();
                IEnumerable<object> p_rest = props.Select(x => x.GetValue(input, null)!);
                var param = p_name.Concat(p_rest).ToArray();
                return (mi.MakeGenericMethod(types).Invoke(null, param) as ICommand)!;
            }


        }
        catch (Exception)
        {
            throw;
        }


    }

    public static bool TryConvert(object input, out ICommand cmd)
    {
        try
        {
            cmd = Convert(input);
            return true;
        }
        catch (Exception)
        {
            cmd = null;
            return false;
        }
    }

}
