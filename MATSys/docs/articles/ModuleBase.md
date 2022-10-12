# ModuleBase class

namespace: <ins><b>MATSys</b></ins> 

## Introduction

## Dependency Injection


## How to operate

## *** Reminder ***
To perfectly implement the Dependenc Injection pattern, ModuleBase uses constructor to inject the <b>IPlugin</b> instance. This means when user inherit from ModuleBase class, it will also implement the ctor. If there's some custom logic that need to peform, we recommend to put the logic in the override <b>Load()</b> method.

> ModuleBase will call **Load()** method in the ctor process, user can put custom logic in the **Load()** method.