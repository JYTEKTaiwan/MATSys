# <ins>M</ins>odular <ins>A</ins>utomated <ins>T</ins>esting <ins>Sys</ins>tem

## Introduction

MATSys is a toolset that helps to accelerate the developing process, including integration, automation scripting, and logging in Test & Measurement industry.

In MATSys, all objects are inherited from a base class called "Module". MATSys gives "Module" 4 built-in features, so user can access these additional features by inheriting the base class. These features are:
1. Logging  
  Flexibility when debuging/tracking/testing  
2. Communication (class name "Transceiver")  
  A REQ/RES communication channel
3. Publishing (class name "Notifier")  
  Use Pub/Sub mechanism to broadcast the result 
4. Data recording (class name "Recorder")  
  Save data locally

All features, except logging, are using dependency injection design. User can inject their own implementation into "Module" object alternatively.
