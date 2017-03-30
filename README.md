## Compiling
You will need Visual Studio 2017. To build from the command line you will need  NodeJS (Tested with 4.1) and Gulp installed.

To get the web IU to build, in Visual Studio, open the `Task Runner Explorer` and run the watch task. You can alternatively do this from the command line: 
```
cd source\Web
gulp watch
```

The TypeScript `ng` errors can be ignored

Run `build.cmd` to do a full rebuild, including tests, before submitting a code-change PR.

