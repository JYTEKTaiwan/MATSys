using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MATSys.Commands
{
    /// <summary>
    /// Attribute that represent the method information for MATSys to use
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MATSysCommandAttribute: Attribute
    {
        /// <summary>
        /// Name of the ICommand instance
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Type of the ICommand instance
        /// </summary>
        public Type? CommandType { get; set; }
        
        public MethodInvoker? Invoker { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="t"></param>
        public MATSysCommandAttribute([CallerMemberName]string Name="", Type? Type=null)
        {           
            Alias = Name;
            CommandType = Type;
        }

        public void ConfigureCommandType(MethodInfo mi)
        {
            if (CommandType == null)
            {
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

        public string GetTemplateString()
        {
            if (CommandType != null)
            {
                var args = CommandType.GenericTypeArguments;
                var sb = new StringBuilder();
                sb.Append(Alias);
                sb.Append("=");
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i]==typeof(string))
                    {
                        sb.Append($"\"{args[i].FullName}\"");
                    }
                    else if (args[i].IsClass)
                    {
                        sb.Append($"{{{args[i].FullName}}}");
                    }
                    else
                    {
                        sb.Append($"{args[i].FullName}");
                    }
                    if (i != args.Length - 1)
                    {
                        sb.Append(",");
                    }
                }
                return sb.ToString();
            }
            else
            {
                throw new ArgumentNullException("null input");
            }


        }


    }
}