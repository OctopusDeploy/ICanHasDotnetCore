module ICanHasDotnetCore.Result {

    export const state = "layout.result";


    export enum SupportType {
        Unknown = 0,
        Supported = 1,
        Unsupported = 2,
        KnownReplacementAvailable = 3,
        InvestigationTarget = 4
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
        wasSuccessful: boolean;
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
            "Building Network"
        ];

        constructor(private $http: ng.IHttpService, $state: ng.ui.IStateService, private $timeout: ng.ITimeoutService) {
            this.setLoadingMessage();

            var packageFiles = <Home.IPackageFile[]>$state.params["data"];
            if (!packageFiles) {
                $state.go(Home.state);
                //this.response = this.testResponse;
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

        private testResponse: IGetResultResponse = {
            graphViz: "",
            result: [
                { packageName: "A", wasSuccessful: true, supportType: SupportType.InvestigationTarget, dependencies: ["B", "C"] },
                { packageName: "B", wasSuccessful: true, supportType: SupportType.Unsupported, dependencies: ["C", "D"] },
                { packageName: "C", wasSuccessful: true, supportType: SupportType.KnownReplacementAvailable, dependencies: ["D"] },
                { packageName: "D", wasSuccessful: true, supportType: SupportType.Supported, dependencies: ["E"] },
                { packageName: "E", wasSuccessful: false, error: "Oops", supportType: SupportType.Unknown, dependencies: [] }
            ]
        };

    }

    addAngularState(state, "/result", "Result", ViewModel, "result/result.html");
}
