module ICanHasDotnetCore.Result.PackageResultBox {
    interface PackageResultBoxScope extends ng.IScope {
        type: string | Number;
        text: string;
    }
      
    app.directive("packageResultBox", (supportTypeService: SupportTypeService.IService) => {

        return <ng.IDirective>{
            restrict: "E",
            scope: {
                type: "=",
                text: "="
            },
            link: (scope: PackageResultBoxScope, element) => {


                var type: SupportType
                if (typeof scope.type === "string") {
                    type = (<any>SupportType)[scope.type];
                } else  {
                    type = <SupportType>scope.type;
                }
                var colours = supportTypeService.getColours(type);

                var el = $(element);
                el.css("background-color", colours.background);
                el.css("border-color", colours.border);
                el.html(scope.text || supportTypeService.getDisplayName(type));
            }
        }
    }
    );

}