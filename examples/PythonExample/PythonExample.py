import sys

from pythonnet import load
load("coreclr")

import clr


sys.path.append(r"C:\Users\JYTW\source\repos\JYTEKTaiwan\MATSys\examples\InteractionWithHost_net472\bin\Debug")

#clr.AddReference("MATSys")
clr.AddReference("Microsoft.Extensions.Hosting")
#clr.AddReference("Microsoft.Extensions.Logging")
#clr.AddReference("Microsoft.Extensions.Logging.EventLog")
clr.AddReference(r"C:\Users\JYTW\.nuget\packages\system.diagnostics.eventlog\6.0.0\lib\net461\System.Diagnostics.EventLog.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.logging\2.1.0\lib\netstandard2.0\Microsoft.Extensions.Logging.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\nlog\5.0.4\lib\netstandard2.0\NLog.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\nlog.extensions.logging\5.0.4\lib\netstandard2.0\NLog.Extensions.Logging.dll")

#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.configuration\6.0.0\lib\net461\Microsoft.Extensions.Configuration.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.configuration.environmentvariables\6.0.0\lib\net461\Microsoft.Extensions.Configuration.EnvironmentVariables.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.configuration.usersecrets\6.0.0\lib\net461\Microsoft.Extensions.Configuration.UserSecrets.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.configuration.commandline\6.0.0\lib\net461\Microsoft.Extensions.Configuration.CommandLine.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.configuration.binder\6.0.0\lib\net461\Microsoft.Extensions.Configuration.Binder.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.dependencyinjection\6.0.0\lib\net461\Microsoft.Extensions.DependencyInjection.dll")
##clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.hosting\6.0.0\lib\net461\Microsoft.Extensions.Hosting.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.logging\6.0.0\lib\net461\Microsoft.Extensions.Logging.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.logging.abstractions\6.0.0\lib\net461\Microsoft.Extensions.Logging.Abstractions.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.logging.configuration\6.0.0\lib\net461\Microsoft.Extensions.Logging.Configuration.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.logging.console\6.0.0\lib\net461\Microsoft.Extensions.Logging.Console.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.logging.debug\6.0.0\lib\net461\Microsoft.Extensions.Logging.Debug.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.logging.eventsource\6.0.0\lib\net461\Microsoft.Extensions.Logging.EventSource.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.logging.eventlog\6.0.0\lib\net461\Microsoft.Extensions.Logging.EventLog.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.options\6.0.0\lib\net461\Microsoft.Extensions.Options.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\microsoft.extensions.options.configurationextensions\6.0.0\lib\net461\Microsoft.Extensions.Options.ConfigurationExtensions.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\system.diagnostics.diagnosticsource\6.0.0\lib\net461\System.Diagnostics.DiagnosticSource.dll")
#clr.AddReference(r"C:\Users\JYTW\.nuget\packages\system.runtime.compilerservices.unsafe\4.5.3\lib\net461\System.Runtime.CompilerServices.Unsafe.dll")



#from MATSys import *
#from MATSys.Commands import *
#from MATSys.Factories import *
#from MATSys.Plugins import *
#from MATSys.Hosting import *
from Microsoft.Extensions.Configuration import *
from Microsoft.Extensions.Hosting import *


#a = RecorderFactory.CreateNew[EmptyRecorder](None);

builder = Host.CreateDefaultBuilder()
#builder=MATSysExtension.UseMATSys(builder)

#cb = ConfigurationBuilder()
#s=FileConfigurationExtensions.SetBasePath(cb,Directory.GetCurrentDirectory())
host=builder.Build();


