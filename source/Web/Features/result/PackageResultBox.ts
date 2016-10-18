module ICanHasDotnetCore.Result.PackageResultBox {
    app.directive("packageResultBox", (supportTypeService: SupportTypeService.IService) => {

        return <ng.IDirective>{
            restrict: "E",
            scope: {
                type: "=",
                text: "="
            },
            link: (scope: ng.IScope, element) => {


                var type = isNaN(Number(scope["type"])) ? SupportType[<string>scope["type"]] : <SupportType>scope["type"];
                var colours = supportTypeService.getColours(type);

                var el = $(element);
                el.css("background-color", colours.background);
                el.css("border-color", colours.border);
                el.html(scope["text"] || supportTypeService.getDisplayName(type));
            }
        }
    }
    );

}