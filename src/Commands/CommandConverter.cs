using System.Reflection;
using MATSys.Commands;

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


        public static ICommand Convert(object input)
        {
            var t = input.GetType();
            //check if input is MATSysCommandContractAttribute
            CheckContract(t);

            var name = t.GetCustomAttribute<MATSysCommandContractAttribute>().MethodName;
            var props = t.GetProperties()
            .Where(x => x.GetCustomAttributes<MATSysCommandOrderAttribute>().Count() != 0)
            .OrderBy(x => x.GetCustomAttribute<MATSysCommandOrderAttribute>().Order).ToList();

            object[] p_name = new object[] { name };
            var mi = m_infos.First(x => x.GetGenericArguments().Count() == props.Count());
            if (props.Count == 0)
            {
                return mi.Invoke(null, p_name) as ICommand;
            }
            else
            {
                Type[] types = props.Select(x => x.PropertyType).ToArray();
                IEnumerable<object> p_rest = props.Select(x => x.GetValue(input));
                var param = p_name.Concat(p_rest).ToArray();
                return mi.MakeGenericMethod(types).Invoke(null, param) as ICommand;
            }

        }
    }
