## Compiling
You will need Visual Studio 2017 or [Jetbrains Rider](https://www.jetbrains.com/rider/) and the [.NET Core 3.0 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0).

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

Deployed by https://deploy.octopushq.com/app#/projects/i-can-has-dotnet-core

Test website is https://icanhasdotnetcore-test.azurewebsites.net/
