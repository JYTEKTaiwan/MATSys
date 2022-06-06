using MATSys.Plugins;
using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public sealed class DataBusFactory : IDataBusFactory
    {
        private readonly Utility.ModuleContext context;
        private const string KEY = "DataBusFactory";
        private Type DefaultType { get; } = typeof(DefaultDataBus);

        public DataBusFactory(IConfiguration configuration)
        {
            context = Utility.ModuleContext.Parse<IDataBus>(configuration, KEY);
            context.TypePrefix = "DataBus";
            //Register assembly resolve event(in case that dynamically loaded assembly had dependent issue)
            AppDomain.CurrentDomain.AssemblyResolve += context.AssemblyResolve;
        }

        public IDataBus CreatePublisher(IConfigurationSection section)
        {
            return context.CreateInstance<IDataBus>(section, DefaultType);
        }
    }
}