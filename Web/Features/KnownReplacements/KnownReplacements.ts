module ICanHasDotnetCore.KnownReplacements {

    export var state = "layout.knownReplacements";

    class ViewModel {
        replacements: any[];

        constructor($http: ng.IHttpService) {
            $http.get<any[]>("/api/KnownReplacements")
                .then(response => this.replacements = response.data);
        }

    }

    addAngularState(state, "/Replacements", "Known Replacements", ViewModel, "KnownReplacements/KnownReplacements.html");
}
