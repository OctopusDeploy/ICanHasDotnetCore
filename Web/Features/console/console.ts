module ICanHasDotnetCore.Console {

    export const state = "layout.console";

    class ViewModel {
    }

    addAngularState(state, "/console", "Command Line Tool", ViewModel, "console/console.html");
}
