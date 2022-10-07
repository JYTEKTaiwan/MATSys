import sys
import clr

sys.path.append(r"C:\Users\JYTW\source\repos\JYTEKTaiwan\MATSys\examples\InteractionWithHost_net472\bin\Debug")

clr.AddReference("MATSys")
clr.AddReference("Microsoft.Extensions.Configuration")
clr.AddReference("Microsoft.Extensions.Hosting")
clr.AddReference("Microsoft.Extensions.Configuration.Abstractions")

from MATSys import *
from MATSys.Commands import *
from MATSys.Factories import *
from MATSys.Plugins import *
from MATSys.Hosting import *
from Microsoft.Extensions.Configuration import *
from Microsoft.Extensions.Hosting import *


a = RecorderFactory.CreateNew[EmptyRecorder](None);

builder = Host.CreateDefaultBuilder()
MATSysExtension.UseMATSys(builder)
builder.Build();

