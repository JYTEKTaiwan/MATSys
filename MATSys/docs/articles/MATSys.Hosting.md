# Embedded to Host

namespace: <ins><b>MATSys.Hosting</b></ins> 

MATSys uses DI(dependency injection) to keep the function flexible. It can also be injected into the .Net generic host ( refer to [.NET Generic Host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)).
Each Module/Transceiver/Notifier/Recorder is a standalone service, running in their own thread.

> A host is an object that encapsulates an app's resources and lifetime functionality

Once MATSys is embeded into Host, system will automatically do the rest:
- Load configuration from appsettings.json in bin folder
- Bring up internal background service to handle all modules
- Start/Stop all modules based on the Host status 

## How to use
Use calls <b>UseMATsys()</b> extended method to add MATSys into the generic Host.like following code snippet
<pre><code>
IHost host = Host.CreateDefaultBuilder().<b>UseMATSys()</b>.Build();
host.RunAsync()
</code></pre>

## How to access to specific handle
Normally we don't need to access the ModuleHubBackgroundService handle. If there's special case, refer to the following line
<pre><code>
var handle = host.Services.GetServices&lt;IHostedService>().OfType&lt;ModuleHubBackgroundService>().FirstOrDefault();
</code></pre>


## How to send the command through ModuleHubBackgroundService
ModuleHubBackgroundService transfer the message to specific Modules in the background, the command string is like this
<pre>
{"Dev1":{"Test":[1,2.0,"hello"]}}
</pre>
 - Dev1 represents the module alias name
 - Test represents the method alias name in the module "Dev1"
 - 1,2.0,"hello" are the parameters of the method "Test", system will these parameter using the method information 

## Advanced feature - Automation Testing
MATSys also provide a scripting featre, which can enable the automated test with pure text scripting.

1. Enable the "ScripMode" property in appsettings.json
2. Editing the script content in the "Scripts" section in appsettings.json
3. Get the runner instance from MATSys service
<pre><code>
var runner = host.Services.GetRunner();
</code></pre>
4. Run the test 
<pre><code>
runner.RunTest();
</code></pre>

The automation test can separated into 3 groups:
- Setup
- Test
- TearDown

Group <b>Setup</b> and group <b>TearDown</b> will execute only once in the beginning of the test and ending of the test. Group <b>Test</b> will execute once or multiple times due to the parameter assigned by user.

Each group can have multiple line of script, so user can implement the test plan by modifying the appsettings.json.

Automation test can be executed use <b>RunTest()</b> method, like following code

<pre><code>
IHost host = Host.CreateDefaultBuilder().UseMATSys().Build();
host.RunAsync().Wait(1000);

var runner = host.Services.GetRunner();
runner.RunTest(3);
</code></pre>

The parameter <b>3</b> in the <b>RunTest(3)</b> means it will execute Setup script one time, then Test script 3 times, and then TearDown script 1 time.

If the parameter is less than 1, system will run infinitely until user call <b>StopTeest()</b>

