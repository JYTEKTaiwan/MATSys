using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MATSys.Commands
{
    /// <summary>
    /// base class to generate command and convert the result.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        private static Regex regex = new Regex(@"^[a-zA-z0-9_]+|[0-9.]+|"".*?""|{.*?}|[a-zA-Z]+");
        internal const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        /// <summary>
        /// Name of the current command
        /// </summary>
        [JsonProperty(Order = 0)]
        public string MethodName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> name</param>
        public CommandBase(string name)
        {
            MethodName = name;
        }

        /// <summary>
        /// Convert the response object to string. This method is optional. Command object will use ToString() is this method is not implemented. Return empty string if <paramref name="obj"/> is null
        /// </summary>
        /// <param name="obj">response from the device object</param>
        /// <returns>result string</returns>
        public virtual string ConvertResultToString(object? obj)
        {
            if (obj == null)
            {
                return "";
            }
            else
            {
                try
                {
                    return JsonConvert.SerializeObject(obj);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        /// <summary>
        /// Get all parameter values
        /// </summary>
        /// <returns>parameter values</returns>
        public abstract object[]? GetParameters();

        /// <summary>
        /// Deserialize the input string into ICommand object
        /// </summary>
        /// <param name="rawString"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ICommand Deserialize(string rawString, Type t)
        {
            try
            {
                if (typeof(ICommand).IsAssignableFrom(t))
                {
                    var cmdInfo = ConvertToJsonFormat(rawString);
                    if (cmdInfo.parameterCount != t.GenericTypeArguments.Length)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    var cmd = JsonConvert.DeserializeObject(cmdInfo.jsonString, t) as ICommand;
                    if (cmd == null)
                    {
                        throw new NullReferenceException("command is null");
                    }
                    return cmd;
                }
                else
                {
                    throw new ArgumentException($"parameter t is not support. {t.Name}");
                }

            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Convert the input string into json format before deserialization
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static (string jsonString, int parameterCount) ConvertToJsonFormat(string input)
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
            return (sb.ToString(), matches.Count - 1);
        }

        /// <summary>
        /// Create command object without any parameters
        /// </summary>
        /// <param name="methodName">name</param>
        /// <returns>Command object</returns>
        public static Command Create(string methodName)
        {
            return new Command(methodName);
        }

        /// <summary>
        /// Serialize the command instance into simplified string format
        /// </summary>
        /// <returns>simplified string</returns>
        public abstract string Serialize();

        /// <summary>
        /// Create command object with 1 parameter assigned
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter</typeparam>
        /// <param name="methodName">name</param>
        /// <param name="param1">parameter value</param>
        /// <returns>Command&lt;<typeparamref name="T1"/>> object </returns>
        public static Command<T1> Create<T1>(string methodName, T1 param1)
        {
            return new Command<T1>(methodName, param1);
        }

        /// <summary>
        /// Create command object with 2 parameters assigned
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter</typeparam>
        /// <typeparam name="T2">Type of the second parameter</typeparam>
        /// <param name="methodName">name</param>
        /// <param name="param1">parameter value</param>
        /// <param name="param2">parameter value</param>
        /// <returns>Command&lt;<typeparamref name="T1"/>,<typeparamref name="T2"/>></returns>
        public static Command<T1, T2> Create<T1, T2>(string methodName, T1 param1, T2 param2)
        {
            return new Command<T1, T2>(methodName, param1, param2);
        }

        /// <summary>
        /// Create command object with 3 parameters assigned
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter</typeparam>
        /// <typeparam name="T2">Type of the second parameter</typeparam>
        /// <typeparam name="T3">Type of the third parameter</typeparam>
        /// <param name="methodName">name</param>
        /// <param name="param1">parameter value</param>
        /// <param name="param2">parameter value</param>
        /// <param name="param3">parameter value</param>
        /// <returns>Command&lt;<typeparamref name="T1"/>,<typeparamref name="T2"/>,<typeparamref name="T3"/>></returns>
        public static Command<T1, T2, T3> Create<T1, T2, T3>(string methodName, T1 param1, T2 param2, T3 param3)
        {
            return new Command<T1, T2, T3>(methodName, param1, param2, param3);
        }

        /// <summary>
        /// Create command object with 4 parameters assigned
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter</typeparam>
        /// <typeparam name="T2">Type of the second parameter</typeparam>
        /// <typeparam name="T3">Type of the third parameter</typeparam>
        /// <typeparam name="T4">Type of the 4th parameter</typeparam>
        /// <param name="methodName">name</param>
        /// <param name="param1">parameter value</param>
        /// <param name="param2">parameter value</param>
        /// <param name="param3">parameter value</param>
        /// <param name="param4">parameter value</param>
        /// <returns>Command&lt;<typeparamref name="T1"/>,<typeparamref name="T2"/>,<typeparamref name="T3"/>,<typeparamref name="T4"/>></returns>
        public static Command<T1, T2, T3, T4> Create<T1, T2, T3, T4>(string methodName, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return new Command<T1, T2, T3, T4>(methodName, param1, param2, param3, param4);
        }

        /// <summary>
        /// Create command object with 5 parameters assigned
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter</typeparam>
        /// <typeparam name="T2">Type of the second parameter</typeparam>
        /// <typeparam name="T3">Type of the third parameter</typeparam>
        /// <typeparam name="T4">Type of the 4th parameter</typeparam>
        /// <typeparam name="T5">Type of the 5th parameter</typeparam>
        /// <param name="methodName">name</param>
        /// <param name="param1">parameter value</param>
        /// <param name="param2">parameter value</param>
        /// <param name="param3">parameter value</param>
        /// <param name="param4">parameter value</param>
        /// <param name="param5">parameter value</param>
        /// <returns>Command&lt;<typeparamref name="T1"/>,<typeparamref name="T2"/>,<typeparamref name="T3"/>,<typeparamref name="T4"/>,<typeparamref name="T5"/>></returns>
        public static Command<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(string methodName, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return new Command<T1, T2, T3, T4, T5>(methodName, param1, param2, param3, param4, param5);
        }

        /// <summary>
        /// Create command object with 6 parameters assigned
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter</typeparam>
        /// <typeparam name="T2">Type of the second parameter</typeparam>
        /// <typeparam name="T3">Type of the third parameter</typeparam>
        /// <typeparam name="T4">Type of the 4th parameter</typeparam>
        /// <typeparam name="T5">Type of the 5th parameter</typeparam>
        /// <typeparam name="T6">Type of the 6th parameter</typeparam>
        /// <param name="methodName">name</param>
        /// <param name="param1">parameter value</param>
        /// <param name="param2">parameter value</param>
        /// <param name="param3">parameter value</param>
        /// <param name="param4">parameter value</param>
        /// <param name="param5">parameter value</param>
        /// <param name="param6">parameter value</param>
        /// <returns>Command&lt;<typeparamref name="T1"/>,<typeparamref name="T2"/>,<typeparamref name="T3"/>,<typeparamref name="T4"/>,<typeparamref name="T5"/>,<typeparamref name="T6"/>></returns>
        public static Command<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(string methodName, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            return new Command<T1, T2, T3, T4, T5, T6>(methodName, param1, param2, param3, param4, param5, param6);
        }

        /// <summary>
        /// Create command object with 7 parameters assigned
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter</typeparam>
        /// <typeparam name="T2">Type of the second parameter</typeparam>
        /// <typeparam name="T3">Type of the third parameter</typeparam>
        /// <typeparam name="T4">Type of the 4th parameter</typeparam>
        /// <typeparam name="T5">Type of the 5th parameter</typeparam>
        /// <typeparam name="T6">Type of the 6th parameter</typeparam>
        /// <typeparam name="T7">Type of the 7th parameter</typeparam>
        /// <param name="methodName">name</param>
        /// <param name="param1">parameter value</param>
        /// <param name="param2">parameter value</param>
        /// <param name="param3">parameter value</param>
        /// <param name="param4">parameter value</param>
        /// <param name="param5">parameter value</param>
        /// <param name="param6">parameter value</param>
        /// <param name="param7">parameter value</param>
        /// <returns>Command&lt;<typeparamref name="T1"/>,<typeparamref name="T2"/>,<typeparamref name="T3"/>,<typeparamref name="T4"/>,<typeparamref name="T5"/>,<typeparamref name="T6"/>,<typeparamref name="T7"/>></returns>
        public static Command<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(string methodName, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        {
            return new Command<T1, T2, T3, T4, T5, T6, T7>(methodName, param1, param2, param3, param4, param5, param6, param7);
        }
    }

    /// <summary>
    /// Command object without any parameter
    /// </summary>
    public sealed class Command : CommandBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Command name</param>
        public Command(string name) : base(name)
        {
        }

        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>
        public override object[]? GetParameters()
        {
            return new object[0];
        }

        /// <summary>
        /// Serialize the cmd object into string
        /// </summary>
        /// <returns>command string</returns>
        public override string Serialize()
        {
            var sb = new StringBuilder();
            sb.Append(MethodName);
            sb.Append("=");
            return sb.ToString();
        }

    }

    /// <summary>
    /// Command object with 1 parameter
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public sealed class Command<T1> : CommandBase
    {
        /// <summary>
        /// Parameters (<see cref="ValueTuple{T1}"/>)
        /// </summary>
        [JsonProperty(Order = 1)]
        public ValueTuple<T1> Parameter { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Command name</param>
        /// <param name="param1"></param>
        public Command(string name, T1 param1) : base(name)
        {
            Parameter = ValueTuple.Create(param1);
        }

        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>
        public override object[]? GetParameters()
        {
            return new object[] { Parameter.Item1! };
        }
        /// <summary>
        /// Serialize the cmd object into string
        /// </summary>
        /// <returns>command string</returns>
        public override string Serialize()
        {
            var sb = new StringBuilder();
            sb.Append(MethodName);
            sb.Append("=");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item1));
            return sb.ToString();
        }

    }

    /// <summary>
    /// Command object with 2 parameters
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public sealed class Command<T1, T2> : CommandBase
    {
        /// <summary>
        /// Parameters (<see cref="ValueTuple{T1,T2}"/>)
        /// </summary>
        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2> Parameter { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Command name</param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        public Command(string name, T1 param1, T2 param2) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2>(param1, param2);
        }
        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>
        public override object[]? GetParameters()
        {
            return new object[] { Parameter.Item1!, Parameter.Item2! };
        }
        /// <summary>
        /// Serialize the cmd object into string
        /// </summary>
        /// <returns>command string</returns>
        public override string Serialize()
        {
            var sb = new StringBuilder();
            sb.Append(MethodName);
            sb.Append("=");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item1));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item2));
            return sb.ToString();

        }

    }

    /// <summary>
    /// Command object with 3 parameters
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public sealed class Command<T1, T2, T3> : CommandBase
    {        /// <summary>
             /// Parameters (<see cref="ValueTuple{T1,T2,T3}"/>)
             /// </summary>

        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3> Parameter { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Command name</param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        public Command(string name, T1 param1, T2 param2, T3 param3) : base(name)
        {
            Parameter = ValueTuple.Create(param1, param2, param3);
        }
        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>

        public override object[]? GetParameters()
        {
            return new object[] { Parameter.Item1!, Parameter.Item2!, Parameter.Item3! };
        }
        /// <summary>
        /// Serialize the cmd object into string
        /// </summary>
        /// <returns>command string</returns>

        public override string Serialize()
        {
            var sb = new StringBuilder();
            sb.Append(MethodName);
            sb.Append("=");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item1));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item2));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item3));

            return sb.ToString();

        }

    }

    /// <summary>
    /// Command object with 4 parameters
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public sealed class Command<T1, T2, T3, T4> : CommandBase
    {
        /// <summary>
        /// Parameters (<see cref="ValueTuple{T1,T2,T3,T4}"/>)
        /// </summary>

        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3, T4> Parameter { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Command name</param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="param4"></param>
        public Command(string name, T1 param1, T2 param2, T3 param3, T4 param4) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2, T3, T4>(param1, param2, param3, param4);
        }
        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>

        public override object[]? GetParameters()
        {
            return new object[] { Parameter.Item1!, Parameter.Item2!, Parameter.Item3!, Parameter.Item4! };
        }
        /// <summary>
        /// Serialize the cmd object into string
        /// </summary>
        /// <returns>command string</returns>

        public override string Serialize()
        {
            var sb = new StringBuilder();
            sb.Append(MethodName);
            sb.Append("=");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item1));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item2));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item3));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item4));
            return sb.ToString();

        }

    }

    /// <summary>
    /// Command object with 5 parameters
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public sealed class Command<T1, T2, T3, T4, T5> : CommandBase
    {
        /// <summary>
        /// Parameters (<see cref="ValueTuple{T1,T2,T3,T4,T5}"/>)
        /// </summary>

        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3, T4, T5> Parameter { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Command name</param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="param4"></param>
        /// <param name="param5"></param>
        public Command(string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2, T3, T4, T5>(param1, param2, param3, param4, param5);
        }
        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>

        public override object[]? GetParameters()
        {
            return new object[] { Parameter.Item1!, Parameter.Item2!, Parameter.Item3!, Parameter.Item4!, Parameter.Item5! };
        }
        /// <summary>
        /// Serialize the cmd object into string
        /// </summary>
        /// <returns>command string</returns>

        public override string Serialize()
        {
            var sb = new StringBuilder();
            sb.Append(MethodName);
            sb.Append("=");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item1));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item2));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item3));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item4));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item5));
            return sb.ToString();

        }

    }

    /// <summary>
    /// Command object with 6 parameters
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    public sealed class Command<T1, T2, T3, T4, T5, T6> : CommandBase
    {
        /// <summary>
        /// Parameters (<see cref="ValueTuple{T1,T2,T3,T4,T5,T6}"/>)
        /// </summary>

        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3, T4, T5, T6> Parameter { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Command name</param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="param4"></param>
        /// <param name="param5"></param>
        /// <param name="param6"></param>
        public Command(string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2, T3, T4, T5, T6>(param1, param2, param3, param4, param5, param6);
        }
        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>

        public override object[]? GetParameters()
        {
            return new object[] { Parameter.Item1!, Parameter.Item2!, Parameter.Item3!, Parameter.Item4!, Parameter.Item5!, Parameter.Item6! };
        }
        /// <summary>
        /// Serialize the cmd object into string
        /// </summary>
        /// <returns>command string</returns>

        public override string Serialize()
        {
            var sb = new StringBuilder();
            sb.Append(MethodName);
            sb.Append("=");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item1));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item2));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item3));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item4));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item5));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item6));
            return sb.ToString();

        }

    }

    /// <summary>
    /// Command object with 7 parameters
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    public sealed class Command<T1, T2, T3, T4, T5, T6, T7> : CommandBase
    {
        /// <summary>
        /// Parameters (<see cref="ValueTuple{T1,T2,T3,T4,T5,T6,T7}"/>)
        /// </summary>

        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3, T4, T5, T6, T7> Parameter { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Command name</param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="param4"></param>
        /// <param name="param5"></param>
        /// <param name="param6"></param>
        /// <param name="param7"></param>
        public Command(string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2, T3, T4, T5, T6, T7>(param1, param2, param3, param4, param5, param6, param7);
        }
        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>

        public override object[]? GetParameters()
        {
            return new object[] { Parameter.Item1!, Parameter.Item2!, Parameter.Item3!, Parameter.Item4!, Parameter.Item5!, Parameter.Item6!, Parameter.Item7! };
        }
        /// <summary>
        /// Serialize the cmd object into string
        /// </summary>
        /// <returns>command string</returns>

        public override string Serialize()
        {
            var sb = new StringBuilder();
            sb.Append(MethodName);
            sb.Append("=");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item1));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item2));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item3));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item4));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item5));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item6));
            sb.Append(",");
            sb.Append(JsonConvert.SerializeObject(Parameter.Item7));
            return sb.ToString();

        }

    }
}