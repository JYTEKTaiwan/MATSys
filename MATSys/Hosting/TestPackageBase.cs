using MATSys.Commands;
using MATSys.Plugins;
using System.Reflection;
using System.Text.Json.Nodes;

namespace MATSys.Hosting
{
    public abstract class TestPackageBase : ITestPackage
    {

        private IServiceProvider _serviceProvider = null;
        private Dictionary<string, MethodInvoker> cmds;
        private INotifier _notifier = new EmptyNotifier();
        public IServiceProvider Provider => _serviceProvider;

        public ITestPackage Base => this;
        INotifier ITestPackage.Notifier => _notifier;
        public Type Type => this.GetType();
        public string Alias { get; set; }
        public TestPackageBase()
        {
            cmds = GetType().GetMethods()
                .Where(x => x.GetCustomAttributes<TestItemParameterAttribute>(false).Count() > 0)
                .Select(x => MethodInvoker.Create(this, x)).ToDictionary(x => x.Name);
        }
        public JsonNode Execute(string testItemName, JsonNode parameter)
        {
            try
            {
                return (JsonNode)cmds[testItemName].Invoke(parameter);
            }
            catch (KeyNotFoundException ex)
            {
                throw ex;
            }

        }
        public void InjectServiceProvider(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }
        public void InjectNotifier(INotifier notifier)
        {
            this._notifier = notifier;
        }
        public void Dispose()
        {

        }
    }
}
