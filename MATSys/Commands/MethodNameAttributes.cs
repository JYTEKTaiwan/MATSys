using Newtonsoft.Json;
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

        //method string should look like this
        // [MethodName]=[Param1],[Param2],...
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
        /// Generate the command string pattern
        /// </summary>
        /// <returns>command string in simplified format</returns>
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

        /// <summary>
        /// Deserialize the input string to ICommand object
        /// </summary>
        /// <param name="str">input string</param>
        /// <returns>ICommand object</returns>
        public ICommand Deserialize(string str)
        {
            var jsonStr = ToJsonString(str);
            var cmd = JsonConvert.DeserializeObject(jsonStr, CommandType) as ICommand;
            return cmd!;
        }
        /// <summary>
        /// Convert the input string into the standard json format string
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>command string in json format</returns>
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