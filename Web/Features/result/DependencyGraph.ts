module ICanHasDotnetCore.Result.DependencyGraph {
    app.directive("dependencyGraph", (supportTypeService: SupportTypeService.IService) => {

        var network;
        var options = {
            nodes: {
                shape: "box",
                size: 25,
                shadow: { enabled: true }
            },
            edges: {
                arrows: { to: true },
                color: {
                    inherit: "to"
                },
                shadow: { enabled: true }
            }
        };



        var createNode = (result: IPackageResult) => (
            {
                id: result.packageName,
                label: result.packageName,
                color: supportTypeService.getColours(result.supportType),
            }
        );

        var createEdge = (from: IPackageResult, to: string) => (
            {
                from: from.packageName,
                to: to
            }
        );

        var setData = (results: IPackageResult[]) => {
            if (!results) {
                network.setData({});
                return;
            }

            var nodes = results.map(createNode);
            var edges = _.flatMap(
                results.filter(f => !!f.dependencies),
                (from) => from.dependencies.map(to => createEdge(from, to))
            );

            network.setData({
                nodes: new vis.DataSet(nodes),
                edges: new vis.DataSet(edges)
            });
        }

        var link = (scope: ng.IScope, element) => {
            scope.$watch("packageResults", setData);
            network = new vis.Network(element[0], {}, options);
        };

        return <ng.IDirective>{
            restrict: "E",
            scope: {
                packageResults: "="
            },
            link: link
        }
    }
    );

}