module ICanHasDotnetCore.Console {

    export const state = "layout.console";

    class ViewModel {
    }

    addAngularState(state, "/Console", "Command Line Tool", ViewModel, "Console/Console.html");
}
