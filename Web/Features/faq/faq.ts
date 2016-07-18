module ICanHasDotnetCore.Faq {

    export const state = "layout.faq";

    class ViewModel {
    }

    addAngularState(state, "/faq", ViewModel, "faq/faq.html",
    {
        title: "FAQ", 
        description: "Frequently Asked Questions"
    });
}
