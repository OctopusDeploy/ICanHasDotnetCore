module ICanHasDotnetCore.Layout {

    class ViewModel {
        version = ICanHasDotnetCore.version;

        constructor() {
        }

    }

    addAngularState("layout", null, null, ViewModel, "layout/layout.html");
}
