namespace MATSys.Hosting.Grpc;


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

