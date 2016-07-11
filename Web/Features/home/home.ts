module ICanHasDotnetCore.Home {

    export const state = "layout_home";
    export interface IPackageFile {
        name?: string;
        file?: any;
    }

    class ViewModel {

        packageFiles: IPackageFile[];
      
        constructor($scope:ng.IScope, private $state: ng.ui.IStateService) {
            this.packageFiles = [{}];

            $scope.$watch("vm.packageFiles", () => this.addPackageFileIfNeeded(), true);
        }

        private addPackageFileIfNeeded() {
            if (this.packageFiles.filter(p => !p.file).length === 0)
                this.packageFiles.push({});
        }

        deletePackageFile(packageFile: IPackageFile) {
            this.packageFiles = _.without(this.packageFiles, packageFile);
        }

        visualiseDependencies() {
            this.$state.go(Result.state,
            {
                 data: this.packageFiles.filter(p => !!p.file)
            });
        }
    }

    addAngularState(state, "/", "Home", ViewModel, "home/home.html");
}
