module ICanHasDotnetCore.Console {

    export const state = "layout.console";

    class ViewModel {
    }

    addAngularState(state, "/Console", ViewModel, "Console/Console.html",
    {
        title: "Command Line Tool",
        description: "Scan a local source tree and generate a report and graph of nuget dependencies and their .NET Standard support"
    });
}
