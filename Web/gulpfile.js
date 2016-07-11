/// <binding AfterBuild='default' />
var gulp = require("gulp");
var $ = require("gulp-load-plugins")({pattern: ["gulp-*", "gulp.*", "del*", "path*"], lazy: true});

var filenames = {
    appJs: "app.js",
    appCss: "app.css",
    vendorJs: "vendor.js",
    vendorCss: "vendor.css"
};
var outputDir = "wwwroot";
var paths = {
    src: {
        appIndex: "index.html",
        appTs: "Features/**/*.ts",
        appLess: ["Features/**/*.less"],
        appNgTemplates: "Features/**/*.html",
        appImages: ["content/images/**"],
        appFonts: [],
        appTsOrder: [
            "**/Features.ts",
            "**/*"
        ],
        dtos: ["Features/**/*Request.cs","Features/**/*Response.cs","../Magic/NugetPackages/SupportType.cs"],
        vendorJs: [
            "node_modules/jquery/dist/jquery.js",
            "node_modules/angular/angular.js",
            "node_modules/angular-ui-router/release/angular-ui-router.js",
            "node_modules/angular-animate/angular-animate.js",
            "node_modules/angular-aria/angular-aria.js",
            "node_modules/angular-material/angular-material.js",
            "node_modules/toastr/build/toastr.min.js",
            "node_modules/lodash/lodash.js",
            "node_modules/ng-file-model/ng-file-model.js",
            "node_modules/vis/dist/vis.js"
        ],
        vendorJsOrder: [
            "**/jquery.js",
            "**/angular.js",
            "**/*"
        ],
        vendorCss: [
            "node_modules/angular-material/angular-material.css",
            "node_modules/toastr/build/toastr.css",
            "node_modules/vis/dist/vis.css"
        ]
    }
};


gulp.task("clean", function (cb) {
    $.del.sync([outputDir], {
        force: true
    });
    cb();
});


gulp.task("vendorScripts", function () {
    return gulp
        .src(paths.src.vendorJs)
        .pipe($.plumber())
        .pipe($.order(paths.src.vendorJsOrder))
        .pipe($.concat(filenames.vendorJs))
        .pipe(gulp.dest(outputDir));
});

gulp.task("appScripts", function () {
    return gulp
        .src(paths.src.appTs)
        .pipe($.sourcemaps.init())
        .pipe($.plumber())
        //.pipe($.order(paths.src.appTsOrder))
        .pipe($.typescript({
            out: filenames.appJs
        }))
        .pipe($.sourcemaps.write())
        .pipe(gulp.dest(outputDir))
        .pipe($.livereload());

});

gulp.task("vendorStyles", function () {
    return gulp
        .src(paths.src.vendorCss)
        .pipe($.plumber())
        .pipe($.flatten())
        .pipe($.concat(filenames.vendorCss))
        .pipe(gulp.dest(outputDir));
});

gulp.task("appStyles", function () {
    return gulp
        .src(paths.src.appLess)
        .pipe($.less())
        .pipe($.plumber())
        .pipe($.flatten())
        .pipe($.concat(filenames.appCss))
        .pipe(gulp.dest(outputDir))
        .pipe($.livereload());

});

gulp.task("ngTemplates", function () {
    return gulp
        .src(paths.src.appNgTemplates)
        .pipe($.plumber())
        .pipe($.bytediff.start())
        .pipe($.htmlmin({
            collapseWhitespace: true,
            conservativeCollapse: true,
            preserveLineBreaks: true
        }))
        .pipe($.bytediff.stop())
        .pipe($.angularTemplatecache({
            root: "app",
            module: "app"
        }))
        .pipe(gulp.dest(outputDir))
        .pipe($.livereload());

});

gulp.task("images", function () {
    return gulp
        .src(paths.src.appImages)
        .pipe($.plumber())
        .pipe(gulp.dest(outputDir + "/images"));
});

gulp.task("fonts", function () {
    return gulp
        .src(paths.src.appFonts)
        .pipe($.plumber())
        .pipe($.flatten())
        .pipe(gulp.dest(outputDir + "/fonts"));
});


gulp.task("app", function (cb) {
    $.runSequence("clean", ["vendorScripts", "appScripts", "vendorStyles", "appStyles", "ngTemplates", "images", "fonts"], cb);
});


gulp.task("debug", ["app"], function () {
    var sources = gulp
        .src([outputDir + "/*.*"])
        .pipe($.order([
            "vendor.js",
            "angular.js",
            "vendor.css",
            "**/*.*",
            "**/*"], {base: outputDir}));

    var timestamp = new Date().getTime();
    
    return gulp
        .src("index.html")
        .pipe($.plumber())
        .pipe($.inject(sources, {
            addRootSlash: true,
            ignorePath: [outputDir],
            addSuffix: "?d=" + timestamp
        }))
        .pipe(gulp.dest(outputDir));
});

gulp.task("release", ["debug"]);

gulp.task("watch", ["debug"], function () {
    $.livereload.listen();
    gulp.watch(paths.src.vendorJs, ['vendorScripts']);
    gulp.watch(paths.src.appTs, ['appScripts']);
    gulp.watch(paths.src.vendorCss, ['vendorStyles']);
    gulp.watch(paths.src.appLess, ['appStyles']);
    gulp.watch(paths.src.appNgTemplates, ['ngTemplates']);
    gulp.watch(paths.src.appImages, ['images']);
});
gulp.task("default", ["debug"]);
