# Factory Design

namespace: <ins><b>MATSys.Factories</b></ins> 

MATSys.Factories Provides 4 factories to use
1. TransceiverFactory
2. NotifierFactory
3. RecorderFactory
4. ModuleFactory

All of these factory are designed to be used in (1) manually creation and (2) embedded in the host service. User can choose the right way to create the instance.

## Factories for plugins (Transceiver/Notifier/Recorder)
Take TransceiverFactory as example(NotifierFactory and RecorderFactory are the same), User can create the factory instance first before create the transceiver instance.
<pre>
// Constructor, use configuration file(appsettings.json) as parameter
<code>public TransceiverFactory(IConfiguration config)</code>

// create the plugin
<code>public ITransceiver CreateTransceiver(IConfigurationSection section)</code>
</pre>

User can also use the static method to create instance. Noted that MATSys allow user to dynamically load the assembly from other directory
<pre>
// static method 
<code>public static ITransceiver CreateNew(string assemblyPath, string typeString, object args)

public static T CreateNew&lt;T>(object args)

public static ITransceiver CreateNew(Type t, object args)</code>
</pre>
Transceiver, Notifier and Recorder instances can be created using these way above.


## Factory for Module
ModuleFactory is slightly different since Module need to have plugins injected in. User can create Module instance by injecting factories or instance as parameters
<pre>
// Constructor for ModuleFactory, inject factories of plulgins as parameters
<code>public ModuleFactory(IConfiguration config, ITransceiverFactory tran_factory, INotifierFactory noti_factory, IRecorderFactory rec_factory)</code>

// Create module
<code>public IModule CreateModule(IConfigurationSection section)</code>
</pre>

ModuleFactory also support static creation
<pre>
//static method, inject plugin instance as parameters
<code>public static IModule CreateNew(string assemblyPath, string typeString, object configuration, ITransceiver transceiver, INotifier notifier, IRecorder recorder, string aliasName = "")

public static T CreateNew&lt;T>(object parameter, ITransceiver? transceiver = null, INotifier? notifier = null, IRecorder? recorder = null, string aliasName = "")

public static IModule CreateNew(Type moduleType, object parameter, ITransceiver? transceiver = null, INotifier? notifier = null, IRecorder? recorder = null, string aliasName = "")</code>
</pre>