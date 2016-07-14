module ICanHasDotnetCore.Result.ResultGroup {

    class ViewModel {

        private foundOnNuget = [
            SupportType.Supported,
            SupportType.PreRelease,
            SupportType.Unsupported
        ];

        type: string;
        allPackages: Result.IPackageResult[];
        packages: Result.IPackageResult[];
        showNugetLink: boolean;

        constructor($scope: ng.IScope) {
            $scope.$watch('vm.allPackages', () => this.updatePackages());
            this.showNugetLink = this.foundOnNuget.indexOf(SupportType[this.type]) >= 0;
        }

        updatePackages() {
            var type = SupportType[this.type];
            this.packages = (this.allPackages || []).filter(p => p.supportType === type);
        }


        getDependencies(pkg: Result.IPackageResult) {
            return this.allPackages
                .filter(p => p.supportType !== SupportType.InvestigationTarget)
                .filter(p => pkg.dependencies.filter(d => p.packageName === d).length > 0);
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