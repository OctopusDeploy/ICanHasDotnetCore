module ICanHasDotnetCore.Statistics {
    import SupportType = ICanHasDotnetCore.Result.SupportType;
    export const state = "layout.statistics";

    interface IResponse {
        statistic: IPackageStatistic;
        moreInformation:any;       
    }

    interface IPackageStatistic {
        name: string;
        count: number;
        latestSupportType: SupportType;
    }

    class ViewModel {
        statistics: IResponse[];
        typeGroups = [ SupportType.Unsupported, SupportType.PreRelease, SupportType.Supported ];

        constructor($http: ng.IHttpService, private supportTypeService: Result.SupportTypeService.IService) {
            $http.get<IResponse[]>("/api/Statistics")
                .then(response => {
                    this.statistics = response.data;
                });
        }

        getSupportTypeName(statistic) {
            return this.supportTypeService.getDisplayName(statistic.latestSupportType);
        }

        statsFor(type: SupportType) {
            return this.statistics.filter(s => s.statistic.latestSupportType === type);
        }
    }

    addAngularState(state, "/Stats", ViewModel, "Statistics/Statistics.html",
    {
        title: "Statistics",
        description: "The most popular packages submitted and their support status"
    });
}
