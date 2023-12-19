using MATSys.Commands;
using System.Reflection;


namespace MATSys.Hosting.Grpc;
/// <summary>
/// Tool used to convert any instance marked with <see cref="MATSysCommandContractAttribute"/> and <see cref="MATSysCommandOrderAttribute"/> into ICommand instance
/// </summary>
public sealed class CommandConverter
{
    //All generic method from CommandBase.Create
    private static Lazy<IEnumerable<MethodInfo>> m_infos => new Lazy<IEnumerable<MethodInfo>>(() => typeof(CommandBase).GetMethods().Where(x => x.Name == "Create"));
    private static MATSysCommandContractAttribute GetContractAttribute(Type t)
    {
        var attr = t.GetCustomAttribute<MATSysCommandContractAttribute>();
        if (attr == null)
        {
            throw new InvalidOperationException("MATSysCommandContract attribute is not presented");
        }
        else
        {
            return attr;
        }
    }


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
            var hasOrderAttr = props.Any(x => x.GetCustomAttributes<MATSysCommandOrderAttribute>().Count() > 0);
            if (hasOrderAttr| !CheckIfRecordType(t))
            {
                props = props
                    .Where(x => x.GetCustomAttributes<MATSysCommandOrderAttribute>().Count() != 0)
                    .OrderBy(x => x.GetCustomAttribute<MATSysCommandOrderAttribute>()!.Order).ToArray();
            }
            
            //Get the method that matches the count of properties
            var mi = m_infos.Value.First(x => x.GetGenericArguments().Count() == props.Count());

            //first argument will be the name of method
            object[] p_name = new object[] { name };

            //return Command instance if no arguments is required
            if (props.Count() == 0) return (mi.Invoke(null, p_name) as ICommand)!;
            else
            {
                // if arguments count is larger than 0, assign the generic method and dynamically create the Command
                Type[] types = props.Select(x => x.PropertyType).ToArray();
                IEnumerable<object> p_rest = props.Select(x => x.GetValue(input)!);
                var param = p_name.Concat(p_rest).ToArray();
                return (mi.MakeGenericMethod(types).Invoke(null, param) as ICommand)!;
            }


        }
        catch (Exception)
        {
            throw;
        }


    }

    private static bool CheckIfRecordType(Type t)
        => ((TypeInfo)t).DeclaredProperties.Any(x => x.Name == "EqualityContract");

}
