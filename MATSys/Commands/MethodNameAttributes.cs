using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MATSys.Commands
{
    /// <summary>
    /// Attribute that represent the method information for MATSys to use
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MethodNameAttribute : Attribute
    {
        /// <summary>
        /// Name of the MATSysCommand object
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Type of the parameter
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
        /// Return the MATSys object json string
        /// </summary>
        /// <returns></returns>
        public string GetJsonString()
        {
            JObject jRoot = new JObject();
            jRoot.Add("MethodName", Name);

            JObject jParam = new JObject();
            var args = CommandType.GenericTypeArguments;
            for (int i = 0; i < args.Length; i++)
            {
                jParam.Add($"Item{i + 1}", args[i].FullName);
            }
            jRoot.Add("Parameter", jParam);

            return jRoot.ToString(Formatting.None);
        }
    }
}