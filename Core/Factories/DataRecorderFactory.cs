using MATSys.Plugins;
using Microsoft.Extensions.Configuration;

namespace MATSys.Factories
{
    public sealed class DataRecorderFactory : IDataRecorderFactory
    {
        private readonly Utility.ModuleContext context;
        private const string KEY = "DataRecorderFactory";
        private Type DefaultType { get; } = typeof(DefaultDataRecorder);

        public DataRecorderFactory(IConfiguration configuration)
        {
            context = Utility.ModuleContext.Parse<IDataRecorder>(configuration, KEY);
            context.TypePrefix = "DataRecorder";
            //Register assembly resolve event(in case that dynamically loaded assembly had dependent issue)
            AppDomain.CurrentDomain.AssemblyResolve += context.AssemblyResolve;
        }

        public IDataRecorder CreateRecorder(IConfigurationSection section)
        {
            return context.CreateInstance<IDataRecorder>(section, DefaultType);
        }
    }
}