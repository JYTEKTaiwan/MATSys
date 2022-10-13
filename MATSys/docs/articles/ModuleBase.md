# ModuleBase class

namespace: <ins><b>MATSys</b></ins> 

## **Introduction**
ModuleBase is an abstract class that integrate most of the operation logics between plugins. User can inherit ModuleBase class and get the well-defined plugins immediately. 
<br></br>

### Plugins
ModuleBase equipped with 3 different types of plugins
- Transceiver (REQ/REP model for command)
- Notifier (Pub/Sub model for reminder)
- Recorder (internal data saving)
<br></br>
### Dependency Injection
Plugins are injected into ModuleBase instance using interface, meanin user can put any implemented instace in the ModuleBase instance. This will maximize the flexibility of the plugins.

Please refer the link for more information: [Dependency Injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

<br></br>

## **How to operate**

<pre><code>
 public class TestDevice : ModuleBase
    {
        public TestDevice(object configuration, ITransceiver server, INotifier bus, IRecorder recorder, string configurationKey = "") : base(configuration, server, bus, recorder, configurationKey)
        {
            //do something in ctor
        }

        public override void Load(IConfigurationSection section)
        {
            //this will be execute during ctor if object can be casted in IConfigurationSection type
        }

        public override void Load(object configuration)
        {
            //this will be execute during ctor if object cannot be casted in IConfigurationSection type
        }


        [MATSysCommand] //Marked as command
        public string Method(string c)
        {
            //custom method
            return c;
        }
    }
</code></pre>


## ***Reminder***
To perfectly implement the Dependency Injection pattern, ModuleBase uses constructor to inject the <b>IPlugin</b> instance. This means when user inherit from ModuleBase class, it will also implement the ctor. If there's some custom logic that need to peform, we recommend to put the logic in the override <b>Load()</b> method.

> ModuleBase will call **Load()** method in the ctor process, user can put custom logic in the **Load()** method.