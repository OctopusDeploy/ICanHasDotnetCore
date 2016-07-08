module ICanHasDotnetCore.Result {

    export const state = "layout.result";


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

        private loadingMessages = [
            "Reticulating Splines",
            "Unpacking Nuggets",
            "Interrogating Byte Server",
            "Building Network",
            "Analysing Big Data",
            "Consulting the Internet of Things"
        ];

        constructor(private $http: ng.IHttpService, $state: ng.ui.IStateService, private $timeout: ng.ITimeoutService, $location: ng.ILocationService) {
            this.setLoadingMessage();

            if ($location.search().demo) {
                $http.get<IGetResultResponse>("/api/GetResult/Demo", {})
                    .then(response => this.response = response.data);
                return;
            }

            var packageFiles = <Home.IPackageFile[]>$state.params["data"];
            if (!packageFiles) {
                $state.go(Home.state);
                return;
            }

            packageFiles = packageFiles.map(f => ({ name: f.name, contents: f.file.data }));
            $http.post<IGetResultResponse>("/api/GetResult", <IGetResultRequest>{ packageFiles: packageFiles })
                .then(response => this.response = response.data);

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

    addAngularState(state, "/result", "Result", ViewModel, "result/result.html");
}
