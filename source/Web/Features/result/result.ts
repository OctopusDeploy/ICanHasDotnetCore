module ICanHasDotnetCore.Result {

    export const state = "layout.result";
    export const demoState = "layout.resultDemo";


    export enum SupportType {
        NotFound = 0,
        Supported = 1,
        PreRelease = 2,
        Unsupported = 3,
        NoDotNetLibraries = 4,
        KnownReplacementAvailable = 5,
        InvestigationTarget = 6,
        Error = 7
    }

    export interface IGetResultRequest {
        packageFiles: Home.IPackageFile[]
    }

    export interface IGetResultResponse {
        graphViz: string;
        result: IPackageResult[];
    }

    export interface IPackageResult {
        packageName: string;
        error?: string;
        supportType: SupportType;
        dependencies: string[];
    }

    const packageFilesStateKey = "packageFiles";


    class ViewModel {

        response: IGetResultResponse;
        loadingMessage: string;
        error: boolean;
        errorMessage: string;

        private loadingMessages = [
            "Reticulating Splines",
            "Unpacking Nuggets",
            "Building Network",
            "Analysing Big Data",
            "Consulting the Internet of Things"
        ];

        constructor(private $http: ng.IHttpService, $state: ng.ui.IStateService, private $timeout: ng.ITimeoutService, $location: ng.ILocationService) {
            this.setLoadingMessage();

            var request: angular.IHttpPromise<IGetResultResponse>;

            if ($state.current.name === demoState) {
                request = $http.post<IGetResultResponse>("/api/GetResult/Demo", {});
            } else if ($state.params["github"]) {
                var data = {
                    id: $state.params["github"]
                };
                request = $http.post<IGetResultResponse>("/api/GetResult/GitHub", data);
            } else if ($state.params["data"]) {
                var packageFiles = <Home.IPackageFile[]>$state.params["data"];

                packageFiles = packageFiles.map(f => ({ name: f.name, contents: f.file.data, originalFileName: f.file.name }));
                request = $http.post<IGetResultResponse>("/api/GetResult", <IGetResultRequest>{ packageFiles: packageFiles });
            } else {
                $state.go(Home.state);
                return;
            }
            request.then(
                response => this.response = response.data,
                response => {
                    this.error = true;
                    if (response.status === 400) {
                        this.errorMessage = response.data;
                    }
                });
        }

        setLoadingMessage() {
            if (this.response)
                return;

            var index = Math.floor(Math.random() * this.loadingMessages.length);
            this.loadingMessage = this.loadingMessages[index];

            this.$timeout(() => this.setLoadingMessage(), 3000);
        }

        hasPackages(typeStr: string) {
            return this.getPackages(typeStr).length > 0;
        }

        getPackages(typeStr: string) {
            if (!this.response)
                return [];
            var type = (<any>SupportType)[typeStr];
            return this.response.result.filter(p => p.supportType === type);
        }

    }

    addAngularState(state, "/result?github", ViewModel, "result/result.html",
    {
        title: "Result", 
        description: "The result of your query",
        },
        { data: null });
    addAngularState(demoState, "/result/demo", ViewModel, "result/result.html",
    {
        title: "Demo",
        description: "Demonstration of the information output after submitting your GitHub repository or package files"
    });
}
