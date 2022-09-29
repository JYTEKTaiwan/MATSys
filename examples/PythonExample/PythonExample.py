import sys
import clr

sys.path.append(r"C:\Users\JYTW\source\repos\JYTEKTaiwan\MATSys\bin\Debug\net472")

clr.AddReference("MATSys")

from MATSys import MATSysHelper

a=MATSysHelper.ListAllModules();

print("R")
