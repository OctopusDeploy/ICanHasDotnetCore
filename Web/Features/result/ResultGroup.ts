module ICanHasDotnetCore.Result.ResultGroup {

    class ViewModel {

        type: string;
        allPackages: Result.IPackageResult[];
        packages: Result.IPackageResult[];

        constructor($scope: ng.IScope) {
            $scope.$watch('vm.allPackages', () => this.updatePackages());
        }

        updatePackages() {
            var type = SupportType[this.type];
            this.packages = (this.allPackages || []).filter(p => p.supportType === type);
        }


        getDependencies(pkg: Result.IPackageResult) {
            return this.allPackages
                .filter(p => pkg.dependencies.filter(d => p.packageName === d).length > 0)
        }
    }

    app.directive("resultGroup", () => {
        return <ng.IDirective>{
            restrict: "E",
            scope: {
                type: "@",
                allPackages: "=",
                description: "@"
            },
            controller: ViewModel,
            controllerAs: "vm",
            bindToController: true,
            templateUrl: "app/result/ResultGroup.html"
        }
    }
    );

}