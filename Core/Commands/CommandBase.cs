//using MACOs.JY.ActorFramework.Core.Devices;
using Newtonsoft.Json;
using System.Reflection;

namespace MATSys.Commands
{
    public abstract class CommandBase : ICommand
    {
        internal const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        [JsonProperty(Order = 0)]
        public string MethodName { get; set; }

        public CommandBase(string name)
        {
            MethodName = name;
        }

        public virtual string? ConvertResultToString(object obj)
        {
            return obj != null ? obj.ToString() : "";
        }

        public abstract string SimplifiedString();

        public abstract object[] GetParameters();

        public static Command Create(string methodName)
        {
            return new Command(methodName);
        }

        public static Command<T1> Create<T1>(string methodName, T1 param1)
        {
            return new Command<T1>(methodName, param1);
        }

        public static Command<T1, T2> Create<T1, T2>(string methodName, T1 param1, T2 param2)
        {
            return new Command<T1, T2>(methodName, param1, param2);
        }

        public static Command<T1, T2, T3> Create<T1, T2, T3>(string methodName, T1 param1, T2 param2, T3 param3)
        {
            return new Command<T1, T2, T3>(methodName, param1, param2, param3);
        }

        public static Command<T1, T2, T3, T4> Create<T1, T2, T3, T4>(string methodName, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return new Command<T1, T2, T3, T4>(methodName, param1, param2, param3, param4);
        }

        public static Command<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(string methodName, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return new Command<T1, T2, T3, T4, T5>(methodName, param1, param2, param3, param4, param5);
        }

        public static Command<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(string methodName, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            return new Command<T1, T2, T3, T4, T5, T6>(methodName, param1, param2, param3, param4, param5, param6);
        }

        public static Command<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(string methodName, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        {
            return new Command<T1, T2, T3, T4, T5, T6, T7>(methodName, param1, param2, param3, param4, param5, param6, param7);
        }

        public Type[] GetParameterTypes()
        {
            return this.GetType().GenericTypeArguments;
        }
    }

    public sealed class Command : CommandBase
    {
        public override string SimplifiedString()
        {
            return $"[{base.MethodName}]";
        }

        public Command(string name) : base(name)
        {
        }

        public override object[] GetParameters()
        {
            return null;
        }
    }

    public sealed class Command<T1> : CommandBase
    {
        [JsonProperty(Order = 1)]
        public ValueTuple<T1> Parameter { get; set; }

        public override string SimplifiedString()
        {
            return $"[{base.MethodName}]: " +
                $"{Parameter.Item1}";
        }

        public Command(string name, T1 param1) : base(name)
        {
            Parameter = ValueTuple.Create(param1);
        }

        public override object[] GetParameters()
        {
            return new object[] { Parameter.Item1 };
        }
    }

    public sealed class Command<T1, T2> : CommandBase
    {
        public override string SimplifiedString()
        {
            return $"[{base.MethodName}]: " +
                $"{Parameter.Item1}," +
                $"{Parameter.Item2}";
        }

        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2> Parameter { get; set; }

        public Command(string name, T1 param1, T2 param2) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2>(param1, param2);
        }

        public override object[] GetParameters()
        {
            return new object[] { Parameter.Item1, Parameter.Item2 };
        }
    }

    public sealed class Command<T1, T2, T3> : CommandBase
    {
        public override string SimplifiedString()
        {
            return $"[{base.MethodName}]: " +
                $"{Parameter.Item1}," +
                $"{Parameter.Item2}," +
                $"{Parameter.Item3}";
        }

        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3> Parameter { get; set; }

        public Command(string name, T1 param1, T2 param2, T3 param3) : base(name)
        {
            Parameter = ValueTuple.Create(param1, param2, param3);
        }

        public override object[] GetParameters()
        {
            return new object[] { Parameter.Item1, Parameter.Item2, Parameter.Item3 };
        }
    }

    public sealed class Command<T1, T2, T3, T4> : CommandBase
    {
        public override string SimplifiedString()
        {
            return $"[{base.MethodName}]: " +
                $"{Parameter.Item1}," +
                $"{Parameter.Item2}," +
                $"{Parameter.Item3}," +
                $"{Parameter.Item4}";
        }

        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3, T4> Parameter { get; set; }

        public Command(string name, T1 param1, T2 param2, T3 param3, T4 param4) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2, T3, T4>(param1, param2, param3, param4);
        }

        public override object[] GetParameters()
        {
            return new object[] { Parameter.Item1, Parameter.Item2, Parameter.Item3, Parameter.Item4 };
        }
    }

    public sealed class Command<T1, T2, T3, T4, T5> : CommandBase
    {
        public override string SimplifiedString()
        {
            return $"[{base.MethodName}]: " +
                $"{Parameter.Item1}," +
                $"{Parameter.Item2}," +
                $"{Parameter.Item3}," +
                $"{Parameter.Item4}," +
                $"{Parameter.Item5}";
        }

        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3, T4, T5> Parameter { get; set; }

        public Command(string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2, T3, T4, T5>(param1, param2, param3, param4, param5);
        }

        public override object[] GetParameters()
        {
            return new object[] { Parameter.Item1, Parameter.Item2, Parameter.Item3, Parameter.Item4, Parameter.Item5 };
        }
    }

    public sealed class Command<T1, T2, T3, T4, T5, T6> : CommandBase
    {
        public override string SimplifiedString()
        {
            return $"[{base.MethodName}]: " +
                $"{Parameter.Item1}," +
                $"{Parameter.Item2}," +
                $"{Parameter.Item3}," +
                $"{Parameter.Item4}," +
                $"{Parameter.Item5}," +
                $"{Parameter.Item6}";
        }

        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3, T4, T5, T6> Parameter { get; set; }

        public Command(string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2, T3, T4, T5, T6>(param1, param2, param3, param4, param5, param6);
        }

        public override object[] GetParameters()
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            return new object[] { Parameter.Item1, Parameter.Item2, Parameter.Item3, Parameter.Item4, Parameter.Item5, Parameter.Item6 };
#pragma warning restore CS8601 // Possible null reference assignment.
        }
    }

    public sealed class Command<T1, T2, T3, T4, T5, T6, T7> : CommandBase
    {
        public override string SimplifiedString()
        {
            return $"[{base.MethodName}]: " +
                $"{Parameter.Item1}," +
                $"{Parameter.Item2}," +
                $"{Parameter.Item3}," +
                $"{Parameter.Item4}," +
                $"{Parameter.Item5}," +
                $"{Parameter.Item6}," +
                $"{Parameter.Item7}";
        }

        [JsonProperty(Order = 1)]
        public ValueTuple<T1, T2, T3, T4, T5, T6, T7> Parameter { get; set; }

        public Command(string name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7) : base(name)
        {
            Parameter = ValueTuple.Create<T1, T2, T3, T4, T5, T6, T7>(param1, param2, param3, param4, param5, param6, param7);
        }

        public override object[] GetParameters()
        {
            return new object[] { Parameter.Item1, Parameter.Item2, Parameter.Item3, Parameter.Item4, Parameter.Item5, Parameter.Item6, Parameter.Item7 };
        }
    }
}