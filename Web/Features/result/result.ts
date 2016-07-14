module ICanHasDotnetCore.Result {

    export const state = "layout.result";
    export const demoState = "layout.resultDemo";


    export enum SupportType {
        NotFound = 0,
        Supported = 1,
        PreRelease = 2,
        Unsupported = 3,
        KnownReplacementAvailable = 4,
        InvestigationTarget = 5,
        Error = 6
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
        loadingMessage;
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
            var type = SupportType[typeStr];
            return this.response.result.filter(p => p.supportType === type);
        }

        private demoResponse: IGetResultResponse = {
            graphViz: "Test data",
            result: [
                { packageName: "A", supportType: SupportType.InvestigationTarget, dependencies: ["B", "C"] },
                { packageName: "B", supportType: SupportType.Unsupported, dependencies: ["C", "D", "F"] },
                { packageName: "C", supportType: SupportType.KnownReplacementAvailable, dependencies: ["D"] },
                { packageName: "D", supportType: SupportType.Supported, dependencies: ["E"] },
                { packageName: "E", error: "Oops, something really really went wrong!!", supportType: SupportType.Error, dependencies: [] },
                { packageName: "F", supportType: SupportType.PreRelease, dependencies: ["G"] },
                { packageName: "G", supportType: SupportType.NotFound, dependencies: [] }
            ]
        };
    }

    addAngularState(state, "/result?github", "Result", ViewModel, "result/result.html");
    addAngularState(demoState, "/result/demo", "Result Demo", ViewModel, "result/result.html");
}
