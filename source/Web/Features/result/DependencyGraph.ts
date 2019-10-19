module ICanHasDotnetCore.Result.DependencyGraph {
    app.directive("dependencyGraph", (supportTypeService: SupportTypeService.IService) => {

        var network: vis.Network;
        var options: vis.Options = {
            nodes: {
                shape: "box",
                size: 25,
                shadow: true
            },
            edges: {
                arrows: { to: true },
                color: {
                    inherit: "to"
                },
                shadow: true
            }
        };

        var getUniqueName = (result: IPackageResult) => {
            if (result.supportType === SupportType.InvestigationTarget) {
                return "Target-" + result.packageName;
            } else {
                return result.packageName;
            }
        }

        var createNode = (result: IPackageResult) => (
            {
                id: getUniqueName(result),
                label: result.packageName,
                color: supportTypeService.getColours(result.supportType),
            }
        );

        var createEdge = (from: IPackageResult, to: string) => (
            {
                from: getUniqueName(from),
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

        var link = (scope: ng.IScope, element: [HTMLElement]) => {
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