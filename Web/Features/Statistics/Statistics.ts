module ICanHasDotnetCore.Statistics {
    import SupportType = ICanHasDotnetCore.Result.SupportType;
    export const state = "layout_statistics";

    interface IPackageStatistic {
        name: string;
        count: number;
        latestSupportType: SupportType;
    }

    class ViewModel {
        statistics: IPackageStatistic[];
        typeGroups = [ SupportType.Unsupported, SupportType.PreRelease, SupportType.Supported ];

        constructor($http: ng.IHttpService, private supportTypeService: Result.SupportTypeService.IService) {
            $http.get<IPackageStatistic[]>("/api/Statistics")
                .then(response => {
                    this.statistics = response.data;
                });
        }

        getSupportTypeName(statistic) {
            return this.supportTypeService.getDisplayName(statistic.latestSupportType);
        }
    }

    addAngularState(state, "/Stats", "Statistics", ViewModel, "Statistics/Statistics.html");
}
