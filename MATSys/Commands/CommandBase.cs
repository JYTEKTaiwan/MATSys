//using MACOs.JY.ActorFramework.Core.Devices;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace MATSys.Commands
{
    /// <summary>
    /// Abstract class to generate command, convert the result.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
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
        /// Convert the response object to string
        /// </summary>
        /// <param name="obj">response from the device object</param>
        /// <returns>conver to string, use ToString() method as default. Return empty string if obj is null</returns>
        public virtual string? ConvertResultToString(object obj)
        {
            return obj != null ? obj.ToString() : "";
        }

        /// <summary>
        /// Get the simplified string for app to use
        /// </summary>
        /// <returns></returns>
        public virtual string SimplifiedString()
        {

            StringBuilder sb=new StringBuilder();
            foreach (var item in GetParameters())
            {
                sb.Append($"{item},");
            }
            var len=sb.Length;
            sb.Remove(len-1,1);
            var str=$"[{MethodName}]: {sb.ToString()}";           
        }


        /// <summary>
        /// Get all parameter values 
        /// </summary>
        /// <returns>parameter values</returns>
        public abstract object[]? GetParameters();

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
        /// Create command object with 1 parameter assigned
        /// </summary>
        /// <typeparam name="T1">Type of the first parameter</typeparam>
        /// <param name="methodName">name</param>
        /// <param name="param1">parameter value</param>
        /// <returns>Command<<typeparamref name="T1"/>> object </returns>
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
        /// <returns>Command<<typeparamref name="T1"/>,<typeparamref name="T2"/>></returns>
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
        /// <returns>Command<<typeparamref name="T1"/>,<typeparamref name="T2"/>,<typeparamref name="T3"/>></returns>
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
        /// <returns>Command<<typeparamref name="T1"/>,<typeparamref name="T2"/>,<typeparamref name="T3"/>,<typeparamref name="T4"/>></returns>
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
        /// <returns>Command<<typeparamref name="T1"/>,<typeparamref name="T2"/>,<typeparamref name="T3"/>,<typeparamref name="T4"/>,<typeparamref name="T5"/>></returns>
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
        /// <returns>Command<<typeparamref name="T1"/>,<typeparamref name="T2"/>,<typeparamref name="T3"/>,<typeparamref name="T4"/>,<typeparamref name="T5"/>,<typeparamref name="T6"/>></returns>
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
        /// <returns>Command<<typeparamref name="T1"/>,<typeparamref name="T2"/>,<typeparamref name="T3"/>,<typeparamref name="T4"/>,<typeparamref name="T5"/>,<typeparamref name="T6"/>,<typeparamref name="T7"/>></returns>
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
        public Command(string name) : base(name)
        {
        }

        /// <summary>
        /// return the parameters in object array
        /// </summary>
        /// <returns></returns>
        public override object[]? GetParameters()
        {
            return null;
        }
    }

    public sealed class Command<T1> : CommandBase
    {
        [JsonProperty(Order = 1)]
        public ValueTuple<T1> Parameter { get; set; }
        
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
    }

    public sealed class Command<T1, T2> : CommandBase
    {
        
        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2> Parameter { get; set; }

        public Command(string name, T1 param1, T2 param2) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2>(param1, param2);
        }

        public override object[]? GetParameters()
        {
            return new object[] { Parameter.Item1!, Parameter.Item2! };
        }
    }

    public sealed class Command<T1, T2, T3> : CommandBase
    {
       
        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3> Parameter { get; set; }

        public Command(string name, T1 param1, T2 param2, T3 param3) : base(name)
        {
            Parameter = ValueTuple.Create(param1, param2, param3);
        }

        public override object[]? GetParameters()
        {
            return new object[] { Parameter.Item1!, Parameter.Item2!, Parameter.Item3! };
        }
    }

    public sealed class Command<T1, T2, T3, T4> : CommandBase
    {
       
        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3, T4> Parameter { get; set; }

        public Command(string name, T1 param1, T2 param2, T3 param3, T4 param4) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2, T3, T4>(param1, param2, param3, param4);
        }

        public override object[]? GetParameters()
        {
            return new object[] { Parameter.Item1!, Parameter.Item2!, Parameter.Item3!, Parameter.Item4! };
        }
    }

    public sealed class Command<T1, T2, T3, T4, T5> : CommandBase
    {
       
        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3, T4, T5> Parameter { get; set; }

        public Command(string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2, T3, T4, T5>(param1, param2, param3, param4, param5);
        }

        public override object[]? GetParameters()
        {
            return new object[] { Parameter.Item1!, Parameter.Item2!, Parameter.Item3!, Parameter.Item4!, Parameter.Item5! };
        }
    }

    public sealed class Command<T1, T2, T3, T4, T5, T6> : CommandBase
    {
        
        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3, T4, T5, T6> Parameter { get; set; }

        public Command(string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2, T3, T4, T5, T6>(param1, param2, param3, param4, param5, param6);
        }

        public override object[]? GetParameters()
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            return new object[] { Parameter.Item1, Parameter.Item2, Parameter.Item3, Parameter.Item4, Parameter.Item5, Parameter.Item6 };
#pragma warning restore CS8601 // Possible null reference assignment.
        }
    }

    public sealed class Command<T1, T2, T3, T4, T5, T6, T7> : CommandBase
    {
        
        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3, T4, T5, T6, T7> Parameter { get; set; }

        public Command(string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2, T3, T4, T5, T6, T7>(param1, param2, param3, param4, param5, param6, param7);
        }

        public override object[]? GetParameters()
        {
            return new object[] { Parameter.Item1!, Parameter.Item2!, Parameter.Item3!, Parameter.Item4!, Parameter.Item5!, Parameter.Item6!, Parameter.Item7! };
        }
    }
}