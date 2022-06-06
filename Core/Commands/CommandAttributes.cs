using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MATSys.Commands
{
    public sealed class CommandObjectAttribute : Attribute
    {
        public string Name { get; }
        public Type CommandType { get; }

        public CommandObjectAttribute(string name, Type t)
        {
            Name = name;
            CommandType = t;
        }

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