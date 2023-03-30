using MATSys.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MATSys.Hosting
{
    public abstract class TestPackageBase : ITestPackage
    {

        private IServiceProvider _serviceProvider = null;
        private Dictionary<string, MethodInvoker> cmds;


        public event EventHandler TestPackageLoaded;
        public event EventHandler TestItemBegins;
        public event EventHandler TestItemSubLoopBegins;
        public event EventHandler TestItemSubLoopCompleted;
        public event EventHandler TestItemCompleted;
        public event EventHandler TestPackageClosed;

        public IServiceProvider Provider => _serviceProvider;

        public string Alias { get; set; }
        public TestPackageBase()
        {
            cmds=GetType().GetMethods()
                .Where(x => x.GetCustomAttributes<TestItemParameterAttribute>(false).Count() > 0)
                .Select(x =>MethodInvoker.Create(this, x)).ToDictionary(x => x.Name);
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

    }
}
