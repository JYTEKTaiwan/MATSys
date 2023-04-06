using MATSys.Commands;
using System.Reflection;
using System.Text.Json.Nodes;

namespace MATSys.Hosting
{
    public abstract class TestPackageBase : ITestPackage
    {

        private IServiceProvider _serviceProvider = null;
        private Dictionary<string, MethodInvoker> cmds;
        public IServiceProvider Provider => _serviceProvider;

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

        public void Dispose()
        {

        }
    }
}
