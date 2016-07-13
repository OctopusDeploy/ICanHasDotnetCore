module ICanHasDotnetCore.Home {

    export const state = "layout.home";
    export interface IPackageFile {
        name?: string;
        file?: any;
    }

    enum Tabs {
        UploadPackageFiles = 0,
        ScanAGitHubRepository = 1
    }

    class ViewModel {

        packageFiles: IPackageFile[];
        selectedTab: Tabs;
        gitHubRepository: string;

        constructor($scope: ng.IScope, private $state: ng.ui.IStateService) {
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
            switch (this.selectedTab) {
                case Tabs.UploadPackageFiles:
                    this.$state.go(Result.state,
                        {
                            data: this.packageFiles.filter(p => !!p.file)
                        });
                    return;
                case Tabs.ScanAGitHubRepository:
                    this.$state.go(Result.state,
                        {
                            github: this.getGitHubRepositoryName()
                        });
                    return;
            }

        }

        canSubmit() {
            switch (this.selectedTab) {
                case Tabs.UploadPackageFiles:
                    return this.packageFiles.length > 1;
                case Tabs.ScanAGitHubRepository:
                    return !!this.getGitHubRepositoryName();
            }
            return false;
        }

        isGitHubRepositoryValid() {
            var repo = this.getGitHubRepositoryName();
            return !!repo &&
                repo.indexOf("/") > 0 &&
                repo.indexOf("/") === repo.lastIndexOf("/");
        }

        getGitHubRepositoryName() {
            var repo = this.gitHubRepository ? this.gitHubRepository.replace("https://github.com/", "") : "";
            return repo.trim().replace(/^\//, "").replace(/\/$/, "");
        }
    }

    addAngularState(state, "/", "Home", ViewModel, "home/home.html");
}
