module ICanHasDotnetCore.Faq {

    export const state = "layout_faq";

    class ViewModel {
    }

    addAngularState(state, "/faq", "Faq", ViewModel, "faq/faq.html");
}
