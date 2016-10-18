module ICanHasDotnetCore.Result.PackageResultBox {
    app.directive("externalLink", ($state, $rootScope: ng.IRootScopeService) => {

        return <ng.IDirective> {
            restrict: "E",
            template: "<a href=\"{{href}}\" target=\"_blank\"><md-icon class=\"external-link\">launch</md-icon></a>",
            replace: true,
            scope: {
                href: "@"
            },
            transclude: true,
            link: (scope: ng.IScope, element, attr, ctrl, transclude) => {
                transclude(clone =>  element.prepend(clone));
            }
        }
    }
    );

}