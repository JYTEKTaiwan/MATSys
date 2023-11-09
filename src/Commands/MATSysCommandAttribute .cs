namespace MATSys.Commands
{
    /// <summary>
    /// Attribute that represent the method information for MATSys to use
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MATSysCommandAttribute : Attribute
    {
        /// <summary>
        /// Name of the ICommand instance
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Type of the ICommand instance
        /// </summary>
        public Type? CommandType { get; set; }

        /// <summary>
        /// MethodInvoker instance
        /// </summary>
        public MethodInvoker? Invoker { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Name">Method name (if not assigned, use original method name instead)</param>
        /// <param name="Type">Type for the Command (if not assigned, user should call ConfigureCommandType to assign manually)</param>
        public MATSysCommandAttribute([CallerMemberName] string Name = "", Type? Type = null)
        {
            Alias = Name;
            CommandType = Type;
        }

        /// <summary>
        /// Configure the command type (bypassed if command type is assigned in the constructor)
        /// </summary>
        /// <param name="mi">MethodInfor instance</param>
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


        /// <summary>
        /// Get the simplified command string        
        /// </summary>
        /// <returns>command string</returns>
        /// <exception cref="ArgumentException">throws for any exception</exception>        
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
                    if (args[i] == typeof(string))
                    {
                        sb.Append($"\"{args[i].FullName}\"");
                    }
                    else if (args[i].IsClass/*class*/|| /*struct*/(args[i].IsValueType && !args[i].IsPrimitive))
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

    }

    /// <summary>
    /// Attributes that indicate the class can be converted to ICommand
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MATSysCommandContractAttribute : Attribute
    {
        /// <summary>
        /// Method Name that used in ICommand
        /// </summary>        
        public string MethodName { get; }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name">name of the method in ICommand</param> 
        public MATSysCommandContractAttribute(string name)
        {

            MethodName = name;
        }
    }
    /// <summary>
    /// Attributes that indicate the property of the instance will be used to convert as parameter in ICommand
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MATSysCommandOrderAttribute : Attribute
    {

        /// <summary>
        /// Order used to create ICommand
        /// </summary>        
        public int Order { get; }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="order">order of the property</param>
        public MATSysCommandOrderAttribute(int order)
        {
            Order = order;
        }
    }

}
