using MATSys.Commands;
using MATSys.Plugins;
using System.Reflection;
using System.Text.Json.Nodes;

namespace MATSys.Hosting
{
    /// <summary>
    /// Abstract TestPackage class that equipped with INotifier and IRecorder to use
    /// </summary>
    public abstract class TestPackageBase : ITestPackage
    {

        private IServiceProvider? _serviceProvider;
        private Dictionary<string, MethodInvoker> cmds = new Dictionary<string, MethodInvoker>();
        private INotifier _notifier = new EmptyNotifier();
        private IRecorder _recorder = new EmptyRecorder();

        /// <summary>
        /// ServiceProvider from the Host
        /// </summary>
        public IServiceProvider? Provider => _serviceProvider;

        /// <summary>
        /// Base object instance
        /// </summary>
        public ITestPackage Base => this;
        INotifier ITestPackage.Notifier => _notifier;
        IRecorder ITestPackage.Recorder => _recorder;
        /// <summary>
        /// Type of the TeatPacakge
        /// </summary>
        public Type Type => this.GetType();
        /// <summary>
        /// Alias of the TeatPacakge
        /// </summary>
        public string? Alias { get; set; }
        /// <summary>
        /// Ctor
        /// </summary>
        public TestPackageBase()
        {
            cmds = GetType().GetMethods()
                .Where(x => x.GetCustomAttributes<TestItemParameterAttribute>(false).Count() > 0)
                .Select(x => MethodInvoker.Create(this, x)).ToDictionary(x => x.Name);
        }

        /// <summary>
        /// Execute the method that marked with <see cref="TestItemParameterAttribute"/>
        /// </summary>
        /// <param name="testItemName">method name</param>
        /// <param name="parameter">parameter in jsonnode format</param>
        /// <returns></returns>
        public IResult Execute(string testItemName, JsonNode parameter)
        {
            try
            {
                var result=cmds[testItemName].Invoke(parameter);
                if (result != null)
                {
                    return (IResult)result;
                }
                else
                {
                    return new TestResult() { Status = ResultStatus.Skip };
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Inject the service provider of Host into the TestPackage
        /// </summary>
        /// <param name="serviceProvider">service provider fronm Host</param>
        public void InjectServiceProvider(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }
        /// <summary>
        /// Inject the INotifier instance
        /// </summary>
        /// <param name="notifier">INotifier instance</param>
        public void InjectNotifier(INotifier notifier)
        {
            this._notifier = notifier;
        }

        /// <summary>
        /// Inject the IRecorder instance
        /// </summary>
        /// <param name="recorder">IRecorder instance</param>
        public void InjectRecorder(IRecorder recorder)
        {
            this._recorder = recorder;
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {

        }
    }


    /// <summary>
    /// Empty TestPackage, used when Runner section or TestPackage section in appsettings.json is empty
    /// </summary>
    public class EmptyTestPackage:TestPackageBase
    {
        
    }
}
