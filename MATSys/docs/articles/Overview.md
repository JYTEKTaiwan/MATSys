# <ins>M</ins>odular <ins>A</ins>utomated <ins>T</ins>esting <ins>Sys</ins>tem

## What's the problem we're facing?
- Complexity of combination of DAQ devices in the OT side (drivers, application,...etc)
- Lack of consideration of re-usable and extendable codes
- A good tool to track the control flow in the background.

## Design Purpose
- Isolate each working instance with its own thread, like a service
- Simplify the coding process by providing a abstract class equipped with pre-defined plugins.
- These plugins can be swapped between buil-in libraries and external custom plugins.
- Introduce powerful open source log tool to increase the debug process
- All of the instance can be defined in json file.

## Key Features for MATSys
- 4 built-in plugins are ready to use after inheriting from the base class    
- Instance can be loaded from configuration file
- Access through string-typed command
- Embedded into .Net generic host, easily to move between applications

## Example


<pre><code>
//Create the host and inject MATSys in it
IHost host = Host.CreateDefaultBuilder().UseMATSys().Build();

//Run the host
host.RunAsync().Wait(1000); ;

//Access the method by send a set of string commands and get response
var response=dev.Execute("Dev1","Test=\"Hello\"");

//Stop the host
host.StopAsync();
</code></pre>

For more examples, please check [Examples](Examples.md)