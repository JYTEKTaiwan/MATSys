import sys

from pythonnet import load
load("coreclr")

import clr
sys.path.append(r"C:\Users\JYTW\source\repos\MATSys\examples\InteractionWithHost\bin\Debug\net6.0-windows")

clr.AddReference("InteractionWithHost");
clr.AddReference("MATSys")

from MATSys import *
from MATSys.Factories import *
from InteractionWithHost import TestDevice



mod=ModuleFactory.CreateNew[TestDevice](None);
res=mod.Execute("{\"Method\":[\"Hello\"]}");
print(res)


