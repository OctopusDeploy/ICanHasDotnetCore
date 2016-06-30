declare var vis;
module ICanHasDotnetCore.Result.DependencyGraph {

    app.directive("DependencyGraph", () => <ng.IDirective>{
            restrict: "E",
            link: (scope: ng.IScope, element, attrs) => {
                scope.$watch(attrs.data,
                    data => {
                        if (data)
                            var network = new vis.Network(element, data, attrs.options);
                    }
                );
            }
        }
    );

}