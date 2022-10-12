# Application Setting File

location: <ins><b>appsettings.json</b></ins> in bin folder

When MATSys is embedded into the .Net generic host, it will automatically read the content of <b>appsettings.json</b> in the bin folder. There are two sections we're gonna to introduce in the following article.
- MATSys section
- NLog section


## MATSys

In the MATSys section , user can assign the following information:

- Reference assembly paths
- Transceiver information for MATSys background service
- Modules information
- Plugins information
- Scripts information


The hierachy may look like this
<pre>
  "MATSys": {    
    "ScriptMode": true,
    "Transceiver": {          },
    "Modules": [          ],
    "References": {    },
    "Scripts": {    }
  } 
</pre>

We're going to take a deep look into it

### References assemblies
MATSys supports loading an external assemblies, so user should assigned the assembly path in the section.
<pre>
    "References": {
      "Modules": [],
      "Recorders": [ ".\\modules\\CSVRecorder.dll" ],
      "Notifiers": [ ".\\modules\\NetMQNotifier.dll" ],
      "Transceivers": [ ".\\modules\\NetMQTransceiver.dll" ]
    }
</pre>
In the <b>References</b> section, user can assign the paths of the assemblies. In general, we recommend to create a folder named <b>"modules"</b> to put all external assemblies in it. 

There's some cases that assemblies might need another depenent assemblie. These dependent assemblies should put in the same folder too.

### Transceiver information for MATSys background service
The background service supports REQ/REP mode to accept command using Transceiver instance. If nothing is assigned, then user can only access the method by having the handle of the instance and direct call.

For more detail of the configuration, please refer to [Plugin information](#Plugininformation).

### Modules information
All the modules in the MATSys system can be created by parsing the information of the <b>Modules</b> section. There are several keys to be explained here:
- Alias
- Type
- Recorder
- Notifier
- Transceiver
- (other custom settings)

Configuration may look like this:

<pre>
    "Modules": [
      {
        "Alias": "Dev1",
        "Type": "TestDevice",
        "Recorder": {  },
        "Notifier": {  },
        "Transceiver": {  }
        //other custom parameters (key-value pair) can be added here

      },
      {
        "Alias": "Dev2",
        "Type": "TestDevice",
      }
    ],

</pre>

<b>Alias</b> is the nickname of the Module instance. user can use different alis name for the same typeof Module. System will identify the Module using Alis properrty.

<b>Type</b> is the type name of the Module, system will use this property to search in the executing assemblies and dynamically create instance for further usage. 

<b>Transceiver</b>, <b>Notifier</b> and <b>Recorder</b> are the plugin information which are injected to the Module, so each module may has different type of plugins. If section is not exist or empty content, system will inject empty plugins into Module.
For more detail of the configuration, please refer to [Plugin information](#Plugininformation).

If there are some unique congifuation setting for custom Module, user can add to the section using key-value pattern.


### Scripts information
MATSys supports automation testing using scripts, user can easily achieve the feature by 2 steps

1. Enable the script mode using </b>ScriptMode</b> in appsettings.json
1. Add script for 3 different groups.

The configuration will be like following snippet
<pre>
    "MATSys": {
        "ScriptMode": true,
        "Scripts": {
            "Setup": [ "Dev1:Test,\"Hello\"" ],
            "Test": [],
            "Teardown": []
    }
  
</pre>

If <b>ScriptMode</b> is disabled, system will only execute command manually.

To be noted that <b>Setup</b> and <b>TearDown</b> are only executed once in the beginning and ending of the whole test.

### Plugin information
Each Module can have its own type of Plugin, so user should fill the configuration setting in the coresponding section.
There are 3 types of plugins
- Transceiver
- Notifier
- Recorder

Property <b>Type</b> in the plugins is verty important. System will use this property to search the right plugins. For example, <b>Type</b> value <u>"csv"</u> in <b>Recorder</b> means system will search "csvRecorder" in the executing assemblie (caption is ignored).

This means if user is going create their own plugins, type name should meet the requirement( xxxRecorder, xxxNotifier, xxxTransceiver)

Like Module, user can assign extra key-value pairs in the section so the custom plugin will take it for configuration.

Take the following snippet as example:

<pre>
    "Notifier": {
      "Type": "netmq",
      "Address": "127.0.0.1:12345",
      "Protocal": "inproc",
      "Topic": "AA",
      "DisableEvent": true
    },
    "Transceiver": {
      "Type": "netmq",
      "LocalIP": "tcp://127.0.0.1",
      "Port": 1234
    }
 
</pre>

System will seach "netmqNotifier" and "netTransceiver" plugin in the executing assemblies and load the key-value pair to configure when initialization.

Take netMQNotifier as exmaple:
<pre><code>
        public void Load(IConfigurationSection section)
        {
            _config = section.Get&lt;NetMQNotifierConfiguration>();

        }

</code></pre>
User can also define a custom class to parse from json section
<pre><code>
    public class NetMQNotifierConfiguration : IMATSysConfiguration
    {
        public string Type { get; set; } = "netmq";
        public bool EnableLogging { get; set; } = false;
        public string Address { get; set; } = "127.0.0.1:5000";
        public string Protocal { get; set; } = "tcp";
        public string Topic { get; set; } = "";
        public bool DisableEvent { get; set; } = true;
    }

</code></pre>

## NLog

MATSys inplement the power tool [Nlog](https://nlog-project.org/) as the logging tool. The configuration can be integrated into appsettings.json.

For more detail of the Nlog setting, please refer to [NLog project site](https://nlog-project.org/)

In MATSys section , if <b>EnableNLogInJsonFile</b> is enabled, then the system will load nlog setting in the appsettings.json.

If disable system will load the NLog.config in the bin folder.
<pre>
  "MATSys": {
    "EnableNLogInJsonFile": true,
  }, 
  "NLog": {
    "throwConfigExceptions": true,
    "targets": {
      "async": true,
      "logfile": {
        "type": "File",
        "fileName": "./${shortdate}.log"
      },
      "logconsole": {
        "type": "Console"
      }
    },
    "rules": [
      {
        "logger": "*",
        "minLevel": "Trace",
        "writeTo": "logfile"
      }
    ]
  }</pre>

  # Example of appsetting.json 
  <pre>
  {
  "MATSys": {
    "EnableNLogInJsonFile": true,
    "ScriptMode": true,
    "Transceiver": {
    },
    "Modules": [
      {
        "Alias": "Dev1",
        "Type": "TestDevice",
        "Recorder": {
          "Type": ""
        },
        "Notifier": {
          "Type": "netmq",
          "Address": "127.0.0.1:12345",
          "Protocal": "inproc",
          "Topic": "AA",
          "DisableEvent": true
        },
        "Transceiver": {
          "Type": "netmq",
          "LocalIP": "tcp://127.0.0.1",
          "Port": 1234
        }
      }
    ],
    "References": {
      "Modules": [],
      "Recorders": [ ".\\modules\\CSVRecorder.dll" ],
      "Notifiers": [ ".\\modules\\NetMQNotifier.dll" ],
      "Transceivers": [ ".\\modules\\NetMQTransceiver.dll" ]
    },
    "Scripts": {
      "Setup": [ "Dev1:Test,\"Hello\"" ],
      "Test": [],
      "Teardown": []
    }
  }, 
  "NLog": {
    "throwConfigExceptions": true,
    "targets": {
      "async": true,
      "logfile": {
        "type": "File",
        "fileName": "./${shortdate}.log"
      },
      "logconsole": {
        "type": "Console"
      }
    },
    "rules": [
      {
        "logger": "*",
        "minLevel": "Trace",
        "writeTo": "logfile"
      }
    ]
  }
}
  </pre>