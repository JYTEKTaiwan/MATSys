using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MATSys.Commands
{
    /// <summary>
    /// Attribute that represent the method information for MATSys to use
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MethodNameAttribute : Attribute
    {
        private Regex regex = new Regex(@"^[a-zA-z0-9_]+|[0-9.]+|"".*?""|{.*?}");
        /// <summary>
        /// Name of the ICommand instance
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Type of the ICommand instance
        /// </summary>
        public Type CommandType { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="t"></param>
        public MethodNameAttribute(string name, Type t)
        {
            Name = name;
            CommandType = t;
        }

        /// <summary>
        /// Return the serialized json string
        /// </summary>
        /// <returns></returns>
        public string GetTemplateString()
        {
            var args = CommandType.GenericTypeArguments;
            var sb = new StringBuilder();
            sb.Append(Name);
            sb.Append("=");
            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i].FullName);
                if (i != args.Length - 1)
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        public ICommand Deserialize(string str)
        {
            var jsonStr = ToJsonString(str);
            var cmd = JsonConvert.DeserializeObject(jsonStr, CommandType) as ICommand;
            return cmd!;
        }
        private string ToJsonString(string input)
        {
            var sb = new StringBuilder();
            var matches = regex.Matches(input);
            var cnt = matches.Count;
            //Prepare header
            sb.Append("{\"MethodName\":\"");
            sb.Append(matches[0].Value);
            sb.Append("\"");
            //Prepare Parameter
            if (cnt != 1)
            {
                //with parameter, continue
                sb.Append(",\"Parameter\":{");
                for (int i = 1; i < cnt; i++)
                {
                    if (i != 1)
                    {
                        sb.Append(",");
                    }
                    sb.Append($"\"Item{i}\":{matches[i].Value}");
                }
                sb.Append("}");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}