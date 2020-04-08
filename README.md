[![NuGet](https://img.shields.io/nuget/v/Quartz.Web.svg)](https://www.nuget.org/packages/Quartz.Web/)

# Quartz.Web

A simple web extension for the great [quartznet](https://github.com/quartznet/quartznet) library.

## Usage

Startup.cs
```
services.AddJobScheduling();
services.AddScopedJob<EchoJob>("0/5 * * * * ?"); //every 5 seconds
```