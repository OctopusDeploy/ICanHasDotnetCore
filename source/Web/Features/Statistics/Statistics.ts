module ICanHasDotnetCore.Statistics {
    export const state = "layout.statistics";

    interface IResponse {
        statistic: IPackageStatistic;
        moreInformation:any;       
    }

    interface IPackageStatistic {
        name: string;
        count: number;
        latestSupportType: Result.SupportType;
    }

    class ViewModel {
        statistics: IResponse[] = [];
        typeGroups = [ Result.SupportType.Unsupported, Result.SupportType.PreRelease, Result.SupportType.Supported ];

        constructor($http: ng.IHttpService, private supportTypeService: Result.SupportTypeService.IService) {
            $http.get<IResponse[]>("/api/Statistics")
                .then(response => {
                    this.statistics = response.data;
                });
        }

        getSupportTypeName(statistic: IPackageStatistic) {
            return this.supportTypeService.getDisplayName(statistic.latestSupportType);
        }

        statsFor(type: Result.SupportType) {
            return this.statistics.filter(s => s.statistic.latestSupportType === type);
        }
    }

    addAngularState(state, "/Stats", ViewModel, "Statistics/Statistics.html",
    {
        title: "Statistics",
        description: "The most popular packages submitted and their support status"
    });
}
