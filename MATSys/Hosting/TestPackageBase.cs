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
    public abstract class TestPackageBase:ITestPackage
    {        
        private readonly string _rootDirectory;
        private readonly IServiceProvider _provider;

        public event EventHandler TestItemBegins;
        public event EventHandler TestItemSubLoopBegins;
        public event EventHandler TestItemSubLoopCompleted;
        public event EventHandler TestItemCompleted;

        public TestPackageBase(IServiceProvider provider=null)
        {
            if (provider!=null)
            {
                //load the configuration 
                _rootDirectory = Assembly.GetExecutingAssembly().Location;
                _provider = provider;
                //
            }
            else
            {

            }
        }

        [TestItemParameter(typeof(ParamType))]
        [MATSysCommand]
        public abstract JsonNode Initialize(JsonNode param);

        [MATSysCommand]
        public abstract JsonNode Close();
    }
    public class ParamType
    {
        public string A { get; set; }
        public int B { get; set; }
    }
    public class A : TestPackageBase
    {
        public A(IServiceProvider provider) : base(provider)
        {
        }

        [MATSysCommand]
        public override JsonNode Close()
        {
            throw new NotImplementedException();
        }

        [TestItemParameter(typeof(ParamType))]
        [MATSysCommand]
        public override JsonNode Initialize(JsonNode param)
        {
            var a=Attribute.GetCustomAttribute(MethodBase.GetCurrentMethod(), typeof(TestItemParameterAttribute)) as TestItemParameterAttribute;
            var aa=param.Deserialize(typeof(ParamType));
            throw new NotImplementedException();
        }
    }
}
