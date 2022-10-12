# Embedded to Host

namespace: <ins><b>MATSys.Hosting</b></ins> 

MATSys uses DI(dependency injection) to keep the function flexible. It can also be injected into the .Net generic host ( refer to [.NET Generic Host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)).
Each Module/Transceiver/Notifier/Recorder is a standalone service, running in their own thread.

> A host is an object that encapsulates an app's resources and lifetime functionality

Once MATSys is embeded into Host, system will automatically do the rest:
- Load configuration from appsettings.json in bin foilder
- Bring up internal background service to handle all modules
- Start/Stop all modules based on the Host status 

## How to use
Use can call <b>UseMATsys()</b> extended method to add MATSys into the generic Host.like following code snippet
<pre><code>
IHost host = Host.CreateDefaultBuilder().<b>UseMATSys()</b>.Build();
host.RunAsync()
</code></pre>

## How to get the handle of the background service
All modules are kept in the backgroun service intance, and user can get backgroun service handle by calling the extended method: 
<pre><code>
var handle = host.Services.GetMATSysHandle();
</code></pre>

## How to send the command
After getting the handle, user can send the command and system will automatically dispatch to coresponding module in the background.
Command look like this
<pre>
Dev1:Test="hello"
</pre>
 - Dev1 represents the module alias name
 - Test represents the method alias name in the module "Dev1"
 - "hello" is the parameter of the method "Test"

Alternatively, background service is also equipped with a transceiver plugin, which means user can send the command using transceiver without having the handle.

## Advanced feature - Automation Testing
Since everything now is in string type, we implement a simple automatic testing feature by script.

Scripts are loaded from the appsettings.json when starting the service. User can call the <b>RunTest()</b> method to execute the automation test. 

The automation test can separated into 3 groups:
- Setup
- Test
- TearDown

Group <b>Setup</b> and group <b>TearDown</b> will execute only once in the beginning of the test and ending of the test. Group <b>Test</b> will execute once or multiple times due to the parameter assigned by user.

Each group can have multiple line of script, so user can implement the test plan by modifying the appsettings.json.

Automation test can be executed use <b>RunTest()</b> method, like following code
<pre><code>
IHost host = Host.CreateDefaultBuilder().UseMATSys().Build();
host.RunAsync().Wait(1000); ;

var dev = host.Services.GetMATSysHandle();
dev.RunTest(3);
</code></pre>
The parameter <b>3</b> in the <b>RunTest(3)</b> means it will execute Setup script one time, then Test script 3 times, and then TearDown script 1 time.

If the parameter is less than 1, system will run infinitely until user call <b>StopTeest()</b>

