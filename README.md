## Compiling
You will need Visual Studio 2017 and dotnet-1.1.1-sdk ([1.1.1 with SDK 1.0.1](https://github.com/dotnet/core/blob/master/release-notes/download-archives/1.1.1-download.md)).  
Note that you can install multiple SDKs side by side, it's all controlled through the global.json.

To build from the command line you will need  NodeJS (Tested with 4.1) and Gulp installed.

To get the web UI to build, in Visual Studio, open the `Task Runner Explorer` and run the watch task. You can alternatively do this from the command line: 
```
cd source\Web
gulp watch
```

The TypeScript `ng` errors can be ignored

Run `build.cmd` to do a full rebuild, including tests, before submitting a code-change PR.

## Deployment

Built on TeamCity https://build.octopushq.com/viewType.html?buildTypeId=Community_ICanHasDotnetCore_BuildICanHasDotnetCore

Deployed by https://deploy3.octopushq.com/app#/projects/i-can-has-dotnet-core

Test website is https://icanhasdotnetcore-test.azurewebsites.net/
