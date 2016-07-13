module ICanHasDotnetCore.Menu {
    app.directive("navMenu", () => {
        return <ng.IDirective>{
            restrict: "E",
            templateUrl: "app/layout/NavMenu.html"
        }
    }
    );
}
