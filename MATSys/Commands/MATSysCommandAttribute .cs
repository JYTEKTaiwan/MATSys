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
        /// Constructor
        /// </summary>
        /// <param name="Name">Method name (if not assigned, use original method name instead)</param>
        /// <param name="Type">Type for the Command (if not assigned, user should call ConfigureCommandType to assign manually)</param>
        public MATSysCommandAttribute([CallerMemberName] string Name = "", Type? Type = null)
        {
            Alias = Name;
            CommandType = Type;
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
