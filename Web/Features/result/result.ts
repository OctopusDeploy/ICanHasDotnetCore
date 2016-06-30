module ICanHasDotnetCore.Result {

    export const state = "layout.result";

    export interface IStateParams extends ng.ui.IStateParamsService
    {
        packageFiles: Home.IPackageFile[]
    }

    export enum SupportType {
        Unknown = 0,
        Supported = 1,
        Unsupported = 2,
        KnownReplacementAvailable = 3,
        InvestigationTarget = 4
    }

    export interface IGetResultResponse {
        result: IPackageResult[];
        graphViz: string;
    }

    export interface IPackageResult {
        packageName: string;
        error: string;
        wasSuccessful: boolean;
        supportType: SupportType;
        dependencies: string[];
    }

    const packageFilesStateKey = "packageFiles";

    class ViewModel {

        constructor(private $http: ng.IHttpService, private $stateParams: IStateParams, $state: ng.ui.IStateService) {
            var packageFiles = $stateParams.packageFiles.map(f => <Dtos.IPackageFile> { name: f.name, contents: f.file.data });
            if (!packageFiles)
                $state.go(Home.state);

            $http.post<Dtos.IGetResultResponse>("/api/GetResult", <Dtos.IGetResultRequest>{ packageFiles: packageFiles })
                .then(response => this.transformResult(response.data));
        }

        transformResult(result: Dtos.IGetResultResponse) {
            
        }
    }

    addAngularState(state, "/result", "Result", ViewModel, "result/result.html");
}
