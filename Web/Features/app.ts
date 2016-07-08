/// <reference path="../typings/main.d.ts" />
declare var vis;
module ICanHasDotnetCore {

    interface IRouteState extends angular.ui.IState {
        params: IRouteParams
    }

    interface IRouteParams extends angular.ui.IStateOptions {
        title: string;
        data: any;
    }

    function httpErrorHandler($httpProvider: ng.IHttpProvider) {
        var interceptor = ($q: ng.IQService) => {
            return {
                'requestError': response => {
                    toastr.error(response.message || response.statusText || "An error occured");
                    return $q.reject(response);
                },
                'responseError': response => {
                    var error = (response.data && (response.data.message || response.data.Message)) || response.statusText || "An error occured";

                    toastr.error(error);
                    return $q.reject(response);
                }
            };
        };
        $httpProvider.interceptors.push(interceptor);
    };

    export var app = angular.module("app", [
        "ng",
        "ui.router",
        "ngMaterial",
        "ng-file-model"
    ])
        .config(httpErrorHandler)
        .config([
            "$httpProvider", $httpProvider => {
                //initialize get if not there
                if (!$httpProvider.defaults.headers.get) {
                    $httpProvider.defaults.headers.get = {};
                }
                //disable IE ajax request caching
                $httpProvider.defaults.headers.get["If-Modified-Since"] = "0";
            }
        ])
        .config(($locationProvider: ng.ILocationProvider) => $locationProvider.html5Mode(true))
        .config(
        ($urlRouterProvider: angular.ui.IUrlRouterProvider) => {
            $urlRouterProvider.when("", "/")
                .otherwise(() => "/");
        }
        )
        .config([
            "$compileProvider", $compileProvider => {
                $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|ftp|mailto|local):/);
            }]
        );

    app.config($mdThemingProvider => {
        $mdThemingProvider.theme('default')
            .primaryPalette('blue')
            .accentPalette('green');
    });

    app.config($mdIconProvider => {
        var rootURL = "ui/images/";

        // Register the user `avatar` icons
        $mdIconProvider
            .icon("menu", rootURL + "menu.svg", 24);
    });

    app.run(($window, $rootScope: ng.IRootScopeService, $location: ng.ILocationService, $http: ng.IHttpService) => {
        $http.get("/api/Analytics")
            .then<string>(result => {
                var sendPageView = () => $window.ga('send', 'pageview', $location.path());

                if (result.data) {
                    $window.ga('create', result.data, 'auto');
                    $rootScope.$on('$stateChangeSuccess', sendPageView);
                    sendPageView();
                }
            });
    });


    export function addAngularState(id: string, url: string, title: string, controller: Function, template: string) {
        var stateConfig: IRouteState = {
            url: url,
            templateUrl: "app/" + template,
            controller: controller,
            controllerAs: "vm",
            params: { title: title, data: null }
        };
        app.config(($stateProvider: angular.ui.IStateProvider) =>
            $stateProvider.state(id, stateConfig));
    }
}