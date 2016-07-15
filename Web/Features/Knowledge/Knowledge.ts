module ICanHasDotnetCore.Knowledge {

    export var state = "layout.knowledge";

    class ViewModel {
        replacements: any[];
        moreInformation: any[];

        constructor($http: ng.IHttpService) {
            $http.get<any[]>("/api/Knowledge/KnownReplacements")
                .then(response => this.replacements = response.data);
            $http.get<any[]>("/api/Knowledge/MoreInformation")
                .then(response => this.moreInformation = response.data);
        }

    }

    redirect("layout.knownReplacements", "/Replacements", state);
    addAngularState(state, "/Knowledge", "Knowledge", ViewModel, "Knowledge/Knowledge.html");
}
