module ICanHasDotnetCore.Faq {

    export const state = "layout.faq";

    class ViewModel {
    }

    addAngularState(state, "/faq", "Faq", ViewModel, "faq/faq.html");
}
