module ICanHasDotnetCore.Result.PackageResultBox {
    app.directive("menuHighlight", ($state, $rootScope: ng.IRootScopeService) => {

        return <ng.IDirective> {
            restrict: "A",
            link: (scope: ng.IScope, element, attr) => {
                var setActive = () => {
                    if ($state.current.name === attr["uiSref"]) {
                        element.addClass("active");
                    } else {
                        element.removeClass("active");
                    }
                };
                $rootScope.$on('$stateChangeSuccess', setActive);
                setActive();
            }
        }
    }
    );

}