using MATSys.Plugins;
using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public sealed class CommandServerFactory : ICommandServerFactory
    {
        private readonly Utility.ModuleContext context;
        private const string KEY = "CommandServerFactory";
        private Type DefaultType { get; } = typeof(DefaultCommandServer);

        public CommandServerFactory(IConfiguration configuration)
        {
            context = Utility.ModuleContext.Parse<ICommandServer>(configuration, KEY);
            context.TypePrefix = "CommandServer";
            //Register assembly resolve event(in case that dynamically loaded assembly had dependent issue)
            AppDomain.CurrentDomain.AssemblyResolve += context.AssemblyResolve;
        }

        public ICommandServer CreateCommandStream(IConfigurationSection section)
        {
            return context.CreateInstance<ICommandServer>(section, DefaultType);
        }
    }
}