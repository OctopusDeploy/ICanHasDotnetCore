/// <reference path="../typings/index.d.ts" />
declare var vis;
module ICanHasDotnetCore {

    interface IRouteState extends angular.ui.IState {
        data: IRouteData
    }

    interface IRouteData {
        title?: string;
        description?: string;
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
        .config(($urlRouterProvider: angular.ui.IUrlRouterProvider) => {
            $urlRouterProvider.when("", "/")
                .otherwise(() => "/");
        })
        .config(($urlMatcherFactoryProvider) => {
            $urlMatcherFactoryProvider.caseInsensitive(true);
            $urlMatcherFactoryProvider.strictMode(false);
        })
        .config([
            "$compileProvider", $compileProvider => {
                $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|ftp|mailto|local):/);
            }]
        );

    app.run(($window, $rootScope: ng.IRootScopeService, $location: ng.ILocationService) => {
        $rootScope.$on('$stateChangeSuccess',
            (evt, toState) => {
                var title = "I Can Has .NET Core";
                var description = "Analyse your nuget dependencies to determine whether they support .NET Standard";
                if (toState.data) {
                    if (toState.data.title) {
                        title = toState.data.title + " - " + title;
                    }
                    if (toState.data.description) {
                        description = toState.data.description;
                    }
                };
                $window.document.title = title;
                $("meta[name='description']").attr("content", description);
                $("meta[property='og:title']").attr("content", title);
                $("meta[property='og:description']").attr("content", description);
            });
    });

    app.config($mdThemingProvider => {
        $mdThemingProvider.theme('default')
            .primaryPalette('blue')
            .accentPalette('orange');
    });

    app.config($mdIconProvider => {
        var rootURL = "ui/images/";

        // Register the user `avatar` icons
        $mdIconProvider
            .icon("menu", rootURL + "menu.svg", 24);
    });

    app.run(($window, $rootScope: ng.IRootScopeService, $location: ng.ILocationService, $http: ng.IHttpService) => {
        $http.get("/api/Analytics")
            .then(result => {
                var sendPageView = () => $window.ga('send', 'pageview', $location.path());

                if (result.data) {
                    $window.ga('create', result.data, 'auto');
                    $rootScope.$on('$stateChangeSuccess', sendPageView);
                    sendPageView();
                }
            });
    });


    export function addAngularState(id: string, url: string, controller: Function, template: string, data: IRouteData, params?: any) {
        var stateConfig: IRouteState = {
            url: url,
            templateUrl: "app/" + template,
            controller: controller,
            controllerAs: "vm",
            data: data,
            params: params
        };
        app.config(($stateProvider: angular.ui.IStateProvider) =>
            $stateProvider.state(id, stateConfig));
    }

    export function redirect(id: string, url: string, redirectToState: string) {
        app.config(($stateProvider: angular.ui.IStateProvider) =>
            $stateProvider.state(id, {
                url: url,
                onEnter: ($state) => $state.go(redirectToState)
            })
        );
    }
}