using System.Text.Json;

namespace MATSys.Commands
{
    /// <summary>
    /// base class to generate command and convert the result.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        internal const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        /// <summary>
        /// json serializer options
        /// </summary>
        protected static JsonSerializerOptions opt = new JsonSerializerOptions()
        {
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        /// <summary>
        /// Name of the current command
        /// </summary>
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
        /// <returns>json formatted string (return empty if parameter "<paramref name="obj"/>" is null)</returns>
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
                    return System.Text.Json.JsonSerializer.Serialize(obj, opt);
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
                    var cmd = JsonSerializer.Deserialize(rawString, t, opt) as ICommand;
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
        public virtual string Serialize()
        {
            return JsonSerializer.Serialize(this, this.GetType(), opt);
        }

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
    [System.Text.Json.Serialization.JsonConverter(typeof(CommandBaseJsonConverter))]
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

    }

    /// <summary>
    /// Command object with 1 parameter
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    [System.Text.Json.Serialization.JsonConverter(typeof(CommandBaseJsonConverter))]
    public sealed class Command<T1> : CommandBase
    {
        /// <summary>
        /// Parameter Type <typeparamref name="T1"/>
        /// </summary>
        public T1 Item1 { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Command name</param>
        /// <param name="param1"></param>
        public Command(string name, T1 param1) : base(name)
        {
            Item1 = param1;
        }

        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>
        public override object[]? GetParameters()
        {
            return new object[] { Item1! };
        }

    }

    /// <summary>
    /// Command object with 2 parameters
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    [System.Text.Json.Serialization.JsonConverter(typeof(CommandBaseJsonConverter))]
    public sealed class Command<T1, T2> : CommandBase
    {
        /// <summary>
        /// Parameter Type <typeparamref name="T1"/>
        /// </summary>
        public T1 Item1 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T2"/>
        /// </summary>
        public T2 Item2 { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Command name</param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        public Command(string name, T1 param1, T2 param2) : base(name)
        {
            Item1 = param1;
            Item2 = param2;
        }
        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>
        public override object[]? GetParameters()
        {
            return new object[] { Item1!, Item2! };
        }

    }

    /// <summary>
    /// Command object with 3 parameters
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    [System.Text.Json.Serialization.JsonConverter(typeof(CommandBaseJsonConverter))]
    public sealed class Command<T1, T2, T3> : CommandBase
    {
        /// <summary>
        /// Parameter Type <typeparamref name="T1"/>
        /// </summary>
        public T1 Item1 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T2"/>
        /// </summary>
        public T2 Item2 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T3"/>
        /// </summary>
        public T3 Item3 { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> Command name</param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        public Command(string name, T1 param1, T2 param2, T3 param3) : base(name)
        {
            Item1 = param1;
            Item2 = param2;
            Item3 = param3;
        }
        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>

        public override object[]? GetParameters()
        {
            return new object[] { Item1!, Item2!, Item3! };
        }

    }

    /// <summary>
    /// Command object with 4 parameters
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    [System.Text.Json.Serialization.JsonConverter(typeof(CommandBaseJsonConverter))]
    public sealed class Command<T1, T2, T3, T4> : CommandBase
    {
        /// <summary>
        /// Parameter Type <typeparamref name="T1"/>
        /// </summary>
        public T1 Item1 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T2"/>
        /// </summary>
        public T2 Item2 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T3"/>
        /// </summary>
        public T3 Item3 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T4"/>
        /// </summary>
        public T4 Item4 { get; set; }

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
            Item1 = param1;
            Item2 = param2;
            Item3 = param3;
            Item4 = param4;
        }
        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>

        public override object[]? GetParameters()
        {
            return new object[] { Item1!, Item2!, Item3!, Item4! };
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
    [System.Text.Json.Serialization.JsonConverter(typeof(CommandBaseJsonConverter))]
    public sealed class Command<T1, T2, T3, T4, T5> : CommandBase
    {
        /// <summary>
        /// Parameter Type <typeparamref name="T1"/>
        /// </summary>
        public T1 Item1 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T2"/>
        /// </summary>
        public T2 Item2 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T3"/>
        /// </summary>
        public T3 Item3 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T4"/>
        /// </summary>
        public T4 Item4 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T5"/>
        /// </summary>
        public T5 Item5 { get; set; }
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
            Item1 = param1;
            Item2 = param2;
            Item3 = param3;
            Item4 = param4;
            Item5 = param5;
        }
        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>

        public override object[]? GetParameters()
        {
            return new object[] { Item1!, Item2!, Item3!, Item4!, Item5! };
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
    [System.Text.Json.Serialization.JsonConverter(typeof(CommandBaseJsonConverter))]
    public sealed class Command<T1, T2, T3, T4, T5, T6> : CommandBase
    {
        /// <summary>
        /// Parameter Type <typeparamref name="T1"/>
        /// </summary>
        public T1 Item1 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T2"/>
        /// </summary>
        public T2 Item2 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T3"/>
        /// </summary>
        public T3 Item3 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T4"/>
        /// </summary>
        public T4 Item4 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T5"/>
        /// </summary>
        public T5 Item5 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T6"/>
        /// </summary>
        public T6 Item6 { get; set; }
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
            Item1 = param1;
            Item2 = param2;
            Item3 = param3;
            Item4 = param4;
            Item5 = param5;
            Item6 = param6;
        }
        /// <summary>
        /// Serialize the cmd object into string
        /// </summary>
        /// <returns>command string</returns>
        public override object[]? GetParameters()
        {
            return new object[] { Item1!, Item2!, Item3!, Item4!, Item5!, Item6! };
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
    [System.Text.Json.Serialization.JsonConverter(typeof(CommandBaseJsonConverter))]
    public sealed class Command<T1, T2, T3, T4, T5, T6, T7> : CommandBase
    {
        /// <summary>
        /// Parameter Type <typeparamref name="T1"/>
        /// </summary>
        public T1 Item1 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T2"/>
        /// </summary>
        public T2 Item2 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T3"/>
        /// </summary>
        public T3 Item3 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T4"/>
        /// </summary>
        public T4 Item4 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T5"/>
        /// </summary>
        public T5 Item5 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T6"/>
        /// </summary>
        public T6 Item6 { get; set; }
        /// <summary>
        /// Parameter Type <typeparamref name="T7"/>
        /// </summary>
        public T7 Item7 { get; set; }

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
            Item1 = param1;
            Item2 = param2;
            Item3 = param3;
            Item4 = param4;
            Item5 = param5;
            Item6 = param6;
            Item7 = param7;
        }
        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>

        public override object[]? GetParameters()
        {
            return new object[] { Item1!, Item2!, Item3!, Item4!, Item5!, Item6!, Item7! };
        }

    }
}