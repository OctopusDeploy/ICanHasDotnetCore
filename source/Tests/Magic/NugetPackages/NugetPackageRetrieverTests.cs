using System;
using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using NuGet;
using NUnit.Framework;
using System.Linq;
using System.Runtime.CompilerServices;
using ApprovalTests;
using ApprovalTests.Reporters;
using ICanHasDotnetCore.NugetPackages;

namespace ICanHasDotnetCore.Tests.Magic.NugetPackages
{
    public class NugetPackageRetrieverTests
    {
        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return CreateTestCase(".NETStandard release package", "Serilog.Sinks.Seq", "2.0.0", SupportType.Supported, "Serilog.Sinks.RollingFile", "Serilog", "Serilog.Sinks.PeriodicBatching");
            yield return CreateTestCase(".NETStandard pre-release package", "Serilog.Sinks.Seq", "2.0.0-rc-57", SupportType.PreRelease, "Serilog.Sinks.RollingFile", "Serilog", "Serilog.Sinks.PeriodicBatching");
            yield return CreateTestCase("Incompatible .NET package", "BootstrapMvcHelpers", "1.0.0", SupportType.Unsupported, "Twitter.Bootstrap");
            yield return CreateTestCase("ASP.NetCore package", "Autofac", "4.0.0-alpha1", SupportType.PreRelease);
            yield return CreateTestCase("DNXCore package", "Autofac", "4.0.0-alpha2", SupportType.PreRelease);
            yield return CreateTestCase(".NETPlatform (dotnet5) package", "structuremap", "4.2.0.402", SupportType.Supported);
            yield return CreateTestCase("Non .NET library", "jQuery", "3.1.0", SupportType.NoDotNetLibraries);
            yield return CreateTestCase("Package doesn't list supported frameworks but is a .NET lib", "Antlr", "3.5.0.2", SupportType.Unsupported);
            yield return CreateTestCase("PCL library", "Microsoft.Azure.Common.Dependencies", "1.0.0", SupportType.Supported, "Microsoft.Bcl", "Microsoft.Bcl.Async", "Microsoft.Bcl.Build", "Microsoft.Net.Http", "Newtonsoft.Json");
            yield return CreateTestCase("Forwarding", "Serilog.Extras.Timing", "2.0.2", SupportType.NoDotNetLibraries, "SerilogMetrics");
            yield return CreateTestCase("OData", "Microsoft.Data.OData", "5.7.0", SupportType.Supported, "System.Spatial", "Microsoft.Data.Edm");
            
        }

       
        public static TestCaseData CreateTestCase(string name, string id, string version, SupportType expectedSupportType, params string[] expectedDependencies)
        {
            var tc = new TestCaseData(id, version, expectedSupportType, expectedDependencies);
            tc.SetName(name);
            return tc;
        }


        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void PackageIsRetrieved(string id, string version, SupportType expectedSupportType, string[] expectedDependencies)
        {
            GetPackage(id, version).Id.Should().Be(id);
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void PackageSupportTypeIsCorrect(string id, string version, SupportType expectedSupportType, string[] expectedDependencies)
        {
            GetPackage(id, version).SupportType.Should().Be(expectedSupportType);
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void PackageDependenciesHaveBeenExtractedCorrectly(string id, string version, SupportType expectedSupportType, string[] expectedDependencies)
        {
            GetPackage(id, version).Dependencies.ShouldAllBeEquivalentTo(expectedDependencies);
        }

        private NugetPackage GetPackage(string id, string version)
        {
            return new NugetPackageInfoRetriever(new PackageRepositoryWrapper(), new NoNugetResultCache())
                 .Retrieve(id, new SemanticVersion(version))
                 .Result;
        }


        [Test]
        public void NonExistantPackageShouldBeNotFound()
        {
            var pkg = GetPackage("FooFooFoo", "1.0.23523");
            pkg.SupportType.Should().Be(SupportType.NotFound);
        }



        [Test]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void CheckTheSupportTypeOfAWholeStackOfPackages()
        {
            var theStack = new Dictionary<string, string>()
            {
                #region The big list of packages
                {"51Degrees.mobi", "3.2.10.3-beta"},
                {"ABCpdf", "10.1.1.1"},
                {"Abot", "1.5.1.42"},
                {"Abp", "0.10.1.2"},
                {"Abp.AutoMapper", "0.10.1.0"},
                {"Abp.EntityFramework", "0.10.1.0"},
                {"Abp.EntityFramework.Common", "0.10.1.0"},
                {"Akka", "1.1.1"},
                {"Akka.Cluster", "1.1.1"},
                {"AndroidSegmentedControl.Droid.Binding", "1.0.2"},
                {"AngleSharp", "0.9.7"},
                {"Angular.UI.Bootstrap", "1.3.3"},
                {"Angular.UI.UI-Router", "0.2.18"},
                {"Angular.UI.Utils", "0.2.3"},
                {"angular-file-upload", "12.0.4"},
                {"AngularGoogleMaps", "2.0.1"},
                {"AngularInjector", "1.0.0"},
                {"angularjs", "1.5.7"},
                {"AngularJS.Animate", "1.5.7"},
                {"Aspose.Words", "16.6.0"},
                {"AsyncErrorHandler.Fody", "1.0.11.0"},
                {"AsyncIO", "0.1.26.0"},
                {"AttributeRouting", "3.5.6"},              
                {"Autofac.Web", "4.0.0"},
                {"DotNetOpenAuth.OAuth.Consumer", "5.0.0-alpha3"},
                {"EnterpriseLibrary.TransientFaultHandling.Configuration", "6.0.1304.0"},
                {"FSharp.Data", "2.3.1"},
                {"FSharp.Data.TypeProviders", "5.0.0.2"},
                {"Glimpse", "2.0.0-beta1"},
                {"glue", "0.9"},
                {"Google.Apis", "1.14.0"},
                {"Google.Apis.Admin.Directory.directory_v1", "1.14.0.561"},
                {"Google.Apis.AndroidPublisher.v2", "1.14.0.543"},
                {"Google.Apis.Auth", "1.14.0"},
                {"Google.Apis.Calendar.v3", "1.14.0.558"},
                {"Google.Apis.Core", "1.14.0"},
                {"Google.Apis.Customsearch.v1", "1.14.0.466"},
                {"Google.GData.Client", "2.2.0.0"},
                {"Google.GData.Extensions", "2.2.0.0"},
                {"Google.ProtocolBuffers", "2.4.1.555"},
                {"googlemaps.TypeScript.DefinitelyTyped", "0.7.1"},
                {"GraphicsMagick.NET-Q16-AnyCPU", "1.3.23.1"},
                {"GraphViz.NET", "1.0.0"},
                {"GravatarHelper", "3.3.2"},
                {"Grid.Mvc", "3.1.0-ci1036"},
                {"Grid.Mvc.DatePicker", "1.0.0"},
                {"Grid.Mvc.Redux", "1.0.4"},
                {"Griddly", "1.5.7.0"},
                {"Halibut", "2.2.3"},
                {"Hammock", "1.3.1"},
                {"handlebars.TypeScript.DefinitelyTyped", "1.1.7"},
                {"Hangfire", "1.6.0"},
                {"Hangfire.AspNetCore", "1.6.0"},
                {"Hangfire.Autofac", "2.2.0"},
                {"Hangfire.Core", "1.6.0"},
                {"Hangfire.Dashboard.Authorization", "2.1.0"},
                {"Hangfire.MemoryStorage", "1.3.0.0"},
                {"Hangfire.Ninject", "1.2.0"},
                {"Hangfire.SimpleInjector", "1.3.0.0"},
                {"Hangfire.SqlServer", "1.6.0"},
                {"Hangfire.StructureMap", "1.6.0"},
                {"Harvest.Chosen", "1.4.2"},
                {"Heijden.Dns", "1.0.0"},
                {"Hekate", "3.2.0"},
                {"Helios", "2.1.2"},
                {"highcharts.TypeScript.DefinitelyTyped", "0.4.3"},
                {"history.TypeScript.DefinitelyTyped", "0.2.7"},
                {"Hopac", "0.2.1"},
                {"html5shiv", "0.1.0.8"},
                {"HtmlAgilityPack", "1.4.9.5"},
                {"HtmlAgilityPack.NetCore", "1.4.9.2"},
                {"HtmlSanitizationLibrary", "1.0.0"},
                {"HtmlSanitizer", "3.3.127-beta"},
                {"Http.fs-prerelease", "3.0.5"},
                {"HttpClientDiagnostics", "1.1.1.1462518862"},
                {"Humanizer", "2.1.0"},
                {"Humanizer.Core", "2.1.0"},
                {"Humanizer.Core.af", "2.1.0"},
                {"Humanizer.Core.de", "2.1.0"},
                {"Hyak.Common", "1.1.0"},
                {"ICSharpCode.SharpZipLib.dll", "0.85.4.369"},
                {"IdentityModel", "2.0.0-beta5"},
                {"IdentityServer.WindowsAuthentication", "1.1.1"},
                {"IdentityServer3", "2.5.1"},
                {"IdentityServer3.AccessTokenValidation", "2.10.0"},
                {"IdentityServer3.AspNetIdentity", "2.0.0"},
                {"IdentityServer3.MembershipReboot", "2.0.0"},
                {"Iesi.Collections", "4.0.1.4000"},
                {"IKVM", "8.1.5717.0"},
                {"ILRepack", "2.1.0-beta1"},
                {"ImageProcessor", "2.4.3.0"},
                {"ImageProcessor.Web", "4.6.3.0"},
                {"ImageProcessor.Web.Config", "2.3.0.0"},
                {"ImageResizer", "4.0.5"},
                {"ImageResizer.Mvc", "4.0.5"},
                {"ImageResizer.MvcWebConfig", "4.0.5"},
                {"ImageResizer.Plugins.AzureReader2", "4.0.5"},
                {"ImageResizer.Plugins.DiskCache", "4.0.5"},
                {"ImageResizer.Plugins.SqlReader", "4.0.5"},
                {"ImageResizer.WebConfig", "4.0.5"},
                {"ImpromptuInterface", "6.2.2"},
                {"InstaSharp", "2.0.4"},
                {"IPAddressRange", "1.6.0.0"},
                {"IppDotNetSdkForQuickBooksApiV3", "2.5.0"},
                {"iTextSharp", "5.5.9"},
                {"itextsharp.pdfa", "5.5.9"},
                {"itextsharp.xmlworker", "5.5.9"},
                {"Janitor.Fody", "1.2.1.0"},
                {"jasmine.TypeScript.DefinitelyTyped", "2.3.2"},
                {"JavaScriptEngineSwitcher.Core", "1.5.0"},
                {"JavaScriptEngineSwitcher.Msie", "1.5.4"},
                {"JavaScriptEngineSwitcher.V8", "1.5.8"},
                {"JetBrains.Annotations", "10.1.5"},
                {"JetBrains.Annotations.Core", "8.2.0.2160"},
                {"Jil", "2.14.3"},
                {"Jint", "2.8"},
                {"jose-jwt", "1.9.3"},
                {"jQuery", "3.1.0"},
                {"jQuery.Ajax.Unobtrusive", "2.0.20710.0"},
                {"jQuery.ba-throttle-debounce", "1.1.0"},
                {"jQuery.BlockUI", "2.70"},
                {"jquery.datatables", "1.10.11"},
                {"jQuery.Easing", "1.3.0.1"},
                {"jQuery.Migrate", "3.0.0"},
                {"jquery.TypeScript.DefinitelyTyped", "3.1.0"},
                {"jQuery.UI.Combined", "1.12.0"},
                {"jquery.validate.unobtrusive.bootstrap", "1.2.3"},
                {"jQuery.Validation", "1.15.0"},
                {"jquery-globalize", "1.1.1"},
                {"jquery-timepicker-jt", "1.9.0"},
                {"json2", "1.0.2"},
                {"JSPool", "0.4.1"},
                {"JWT", "1.3.4"},
                {"Knockout.Mapping", "2.4.0"},
                {"Knockout.Validation", "2.0.3"},
                {"knockoutjs", "3.4.0"},
                {"libphonenumber-csharp", "7.2.5"},
                {"LibSassHost", "0.5.2"},
                {"Libuv", "1.9.0"},
                {"LightInject", "4.0.11"},
                {"LightningDB", "0.9.4"},
                {"LinqKit", "1.1.7.0"},
                {"LinqToQuerystring", "0.7.0.8"},
                {"LoadAssembliesOnStartup.Fody", "1.7.0"},
                {"lodash", "4.13.1"},
                {"log4net", "2.0.5"},
                {"log4net.Raygun.WebApi", "4.2.0.18163"},
                {"logentries.core", "2.8.0"},
                {"logentries.log4net", "2.8.0"},
                {"logentries.nlog", "2.6.0"},
                {"loggly-csharp", "4.5.1.11"},
                {"loggly-csharp-config", "4.5.1.11"},
                {"Lu.AspnetIdentity.Dapper", "0.0.3.3"},
                {"Lucene.Net", "3.0.3"},
                {"Magnum", "2.1.3"},
                {"MahApps.Metro", "1.3.0-ALPHA178"},
                {"MailChimp.NET", "1.1.88"},
                {"MailKit", "1.4.1"},
                {"Markdown", "1.14.7"},
                {"MarkdownSharp", "1.13.0.0"},
                {"Marten", "1.0.0-alpha-625"},
                {"MassTransit", "3.3.5"},
                {"MassTransit.RabbitMQ", "3.3.5"},
                {"MaterialDesignColors", "1.1.3"},
                {"MaterialDesignThemes", "2.0.1-ci644"},
                {"MaterialDesignThemes.MahApps", "0.0.6"},
                {"MathNet.Numerics", "3.12.0"},
                {"MaxMind.Db", "2.1.1-beta1"},
                {"MaxMind.GeoIP2", "2.7.0-beta2"},
                {"MediatR", "2.1.0"},
                {"MediaTypeMap", "2.1.0.0"},
                {"Metrics.NET", "0.4.0-pre"},
                {"MicosoftReportViewerWebForms_v11", "1.0.1"},
                {"MicroBuild.Core", "0.2.0"},
                {"Microsoft.AnalyzerPowerPack", "1.1.0"},
                {"Microsoft.ApplicationInsights", "2.2.0-beta1"},
                {"Microsoft.ApplicationInsights.Agent.Intercept", "2.0.1"},
                {"Microsoft.ApplicationInsights.AspNetCore", "1.0.0"},
                {"Microsoft.ApplicationInsights.DependencyCollector", "2.2.0-beta1"},
                {"Microsoft.ApplicationInsights.Log4NetAppender", "2.1.0"},
                {"Microsoft.ApplicationInsights.NLogTarget", "2.1.0"},
                {"Microsoft.ApplicationInsights.TraceListener", "2.1.0"},
                {"Microsoft.ApplicationInsights.Web", "2.2.0-beta1"},
                {"Microsoft.AspNetCore.Antiforgery", "1.0.0"},
                {"Microsoft.AspNetCore.Authentication", "1.0.0"},
                {"Microsoft.AspNetCore.Authentication.Cookies", "1.0.0"},
                {"Microsoft.AspNetCore.Authentication.OpenIdConnect", "1.0.0"},
                {"Microsoft.AspNetCore.Authorization", "1.0.0"},
                {"Microsoft.AspNetCore.Cors", "1.0.0"},
                {"Microsoft.Azure.DocumentDB", "1.9.1"},
                {"Microsoft.Azure.KeyVault.Core", "1.0.0"},
                {"Microsoft.Azure.NotificationHubs", "1.0.6"},
                {"Microsoft.Azure.Search", "1.1.2"},
                {"Microsoft.Extensions.Logging.Debug", "1.0.0"},
                {"MimeKit", "1.4.1"},
                {"Mindscape.Raygun4Net", "5.3.1"},
                {"Nancy", "2.0.0-barneyrubble"},
                {"Ninject", "3.2.3-unstable-012"},
                {"NuGet.LibraryModel", "3.5.0-beta2-1484"},
                {"NuGet.PackageNPublish", "0.8.0.2"},
                {"NuGet.Packaging", "3.5.0-beta2-1484"},
                {"NuGet.Packaging.Core", "3.5.0-beta2-1484"},
                {"NuGet.Packaging.Core.Types", "3.5.0-beta2-1484"},
                {"NuGet.ProjectModel", "3.5.0-beta2-1484"},
                {"NuGet.Protocol.Core.Types", "3.5.0-beta2-1484"},
                {"NuGet.Protocol.Core.v3", "3.5.0-beta2-1484"},
                {"NuGet.Repositories", "3.5.0-beta2-1484"},
                {"NuGet.RuntimeModel", "3.5.0-beta2-1484"},
                {"NuGet.Versioning", "3.5.0-beta2-1484"},
                {"NUnit", "3.4.1"},
                {"NUnit.Console", "3.4.1"},
                {"NUnit.ConsoleRunner", "3.4.1"},
                {"NUnit.Extension.NUnitProjectLoader", "3.4.1"},
                {"NUnit.Extension.NUnitV2Driver", "3.4.1"},
                {"NUnit.Extension.NUnitV2ResultWriter", "3.4.1"},
                {"NUnit.Extension.TeamCityEventListener", "1.0.1"},
                {"NUnit.Extension.VSProjectLoader", "3.4.1"},
                {"NUnit.Runners", "3.4.1"},
                {"NUnit3TestAdapter", "3.4.0"},
                {"NUnitLite", "3.4.1"},
                {"NUnitTestAdapter", "2.0.0"},
                {"nuPickers", "1.5.3"},
                {"Nustache", "1.16.0.1"},
                {"OAuth2", "0.8.37"},
                {"Octokit", "0.20.0"},
                {"OctoPack", "3.0.71"},
                {"Octopus.Client", "3.4.0-beta0002"},
                {"Octostache", "1.0.2.40"},
                {"Oracle.ManagedDataAccess", "12.1.24160419"},
                {"Our.Umbraco.CoreValueConverters", "3.0.0"},
                {"Our.Umbraco.NestedContent", "0.3.0"},
                {"OwinRequestScopeContext", "1.0.1"},
                {"PagedList", "1.17.0.0"},
                {"PagedList.Mvc", "4.5.0.0"},
                {"PCLCrypto", "2.0.147"},
                {"PCLStorage", "1.0.2"},
                {"PDFsharp-MigraDoc-gdi", "1.50.4000-beta3b"},
                {"PhantomJS", "2.1.1"},
                {"PInvoke.BCrypt", "0.3.2"},
                {"PInvoke.Kernel32", "0.3.2"},
                {"PInvoke.NCrypt", "0.3.2"},
                {"PInvoke.Windows.Core", "0.3.2"},
                {"Platform.NET", "1.2.1.283"},
                {"Platform.Xml.Serialization", "1.2.1.283"},
                {"Polly", "4.3.0"},
                {"Portable.BouncyCastle", "1.8.1.1"},
                {"Portable.Licensing", "1.1.0"},
                {"PortableRest.Signed", "3.1.0-Beta4"},
                {"Postal.Mvc5", "1.2.0"},
                {"PostSharp", "4.3.13-rc"},
                {"Prism.Core", "6.2.0-pre1"},
                {"Prism.Wpf", "6.1.1-pre2"},
                {"PropertyChanged.Fody", "1.51.3"},
                {"protobuf-net", "2.1.0"},
                {"PubSub", "1.5.0"},
                {"Punchclock", "1.2.0"},
                {"Q", "2.0.2-experimental"},
                {"Quartz", "2.3.3"},
                {"RabbitMQ.Client", "3.6.3"},
                {"RavenDB.Client", "3.5.0-rc-35160"},
                {"RavenDB.Client.Authorization", "3.5.0-rc-35160"},
                {"RazorEngine", "4.4.0-rc1"},
                {"RazorGenerator.MsBuild", "2.4.5"},
                {"RazorGenerator.Mvc", "2.4.2"},
                {"React.Core", "2.5.0"},
                {"react.js", "0.14.7"},
                {"React.Web", "2.5.0"},
                {"React.Web.Mvc4", "2.5.0"},
                {"reactiveui", "6.5.0"},
                {"ReactiveUI.Fody", "1.1.51"},
                {"reactiveui-androidsupport", "6.5.0"},
                {"reactiveui-core", "6.5.0"},
                {"reactiveui-events", "6.5.0"},
                {"recaptcha", "1.0.5.0"},
                {"RecaptchaNet", "2.1.0"},
                {"RedisAspNetProviders", "1.0.1"},
                {"RefactorThis.GraphDiff", "2.0.1"},
                {"refit", "2.4.1"},
                {"Remotion.Linq", "2.1.1"},
                {"ReportViewerCommon", "11.0.0.0"},
                {"RequireJS", "2.2.0"},
                {"requirejs.TypeScript.DefinitelyTyped", "0.4.3"},
                {"ReSharper.Annotations", "7.1.3.130415"},
                {"Respond", "1.4.2"},
                {"RestSharp", "105.2.3"},
                {"RhinoMocks", "4.0.0-alpha3"},
                {"RJP.UmbracoMultiUrlPicker", "1.3.1"},
                {"RollbarSharp", "1.0.0.0"},
                {"Rotativa", "1.7.1-beta"},
                {"routedebugger", "2.1.5"},
                {"Rssdp", "1.0.0.12"},
                {"runtime.native.System", "4.0.0"},
                {"runtime.native.System.Net.Http", "4.0.1"},
                {"runtime.native.System.Security.Cryptography", "4.0.0"},
                {"Ruzzie.Common", "0.6.26"},
                {"Sammy.js", "0.7.5"},
                {"Select2.js", "4.0.3"},
                {"Selenium.Support", "2.53.1"},
                {"Selenium.WebDriver", "2.53.1"},
                {"Selenium.WebDriver.ChromeDriver", "2.22.0.0"},
                {"semver", "1.1.2"},
                {"Sendgrid", "7.1.1"},
                {"SendGrid.CSharp.HTTP.Client", "2.0.7"},
                {"SendGrid.SmtpApi", "1.3.1"},
                {"Seq.Client.NLog", "2.3.14"},
                {"Serilog", "2.1.0-dev-00670"},
                {"Serilog.Enrichers.Thread", "2.0.0"},
                {"Serilog.Extensions.Logging", "1.1.0-dev-10116"},
                {"Serilog.Extras.MSOwin", "2.0.1"},
                {"Serilog.Extras.Timing", "2.0.2"},
                {"Serilog.Extras.Topshelf", "2.0"},
                {"Serilog.Settings.AppSettings", "2.0.0"},
                {"Serilog.Sinks.Elasticsearch", "4.0.142"},
                {"Serilog.Sinks.File", "2.1.0"},
                {"Serilog.Sinks.Literate", "2.1.0-dev-00028"},
                {"Serilog.Sinks.MSSqlServer", "4.0.0"},
                {"Serilog.Sinks.PeriodicBatching", "2.0.0"},
                {"Serilog.Sinks.RollingFile", "2.1.0"},
                {"Serilog.Sinks.Seq", "3.0.0-dev-00069"},
                {"Serilog.Sinks.Stackify", "1.23.1-beta2"},
                {"Serilog.Sinks.Trace", "2.0.0"},
                {"SerilogMetrics", "1.0.34"},
                {"SerilogWeb.Classic", "2.0.9"},
                {"SerilogWeb.Owin", "1.0.1"},
                {"ServerAppFabric.Client", "1.1.2106.32"},
                {"ServiceStack", "4.0.60"},
                {"ServiceStack.Api.Swagger", "4.0.60"},
                {"ServiceStack.Client", "4.0.60"},
                {"ServiceStack.Common", "4.0.60"},
                {"ServiceStack.Interfaces", "4.0.60"},
                {"ServiceStack.Logging.Elmah", "4.0.60"},
                {"ServiceStack.Logging.NLog", "4.0.60"},
                {"ServiceStack.OrmLite", "4.0.60"},
                {"ServiceStack.OrmLite.SqlServer", "4.0.60"},
                {"ServiceStack.Redis", "4.0.60"},
                {"ServiceStack.Server", "4.0.60"},
                {"ServiceStack.Text", "4.0.60"},
                {"SevenZipSharp", "0.64"},
                {"Shaolinq", "1.1.0.924"},
                {"Shaolinq.Sqlite", "1.1.0.924"},
                {"Shaolinq.SqlServer", "1.1.0.924"},
                {"SharpBITS", "2.1.0"},
                {"SharpSerializer", "2.20"},
                {"SharpZipLib", "0.86.0"},
                {"Shouldly", "2.8.0"},
                {"Sigil", "4.7.0"},
                {"signalr.TypeScript.DefinitelyTyped", "0.4.1"},
                {"SimpleInjector", "3.2.0"},
                {"SimpleInjector.Extensions.ExecutionContextScoping", "3.2.0"},
                {"SimpleInjector.Extensions.LifetimeScoping", "3.2.0"},
                {"SimpleInjector.Integration.Web", "3.2.0"},
                {"SimpleInjector.Integration.Web.Mvc", "3.2.0"},
                {"SimpleInjector.Integration.WebApi", "3.2.0"},
                {"SimpleInjector.Integration.WebApi.WebHost.QuickStart", "3.2.0"},
                {"SimpleInjector.Packaging", "3.2.0"},
                {"SimpleJson", "0.38.0"},
                {"SlowCheetah", "2.5.15"},
                {"Soda.GoogleAnalytics", "1.0.4491.30130"},
                {"Soda.GoogleAnalytics.MVC", "1.0.4491.29854"},
                {"SonarAnalyzer.CSharp", "1.15.0"},
                {"SourceLink.Fake", "1.1.0"},
                {"Spin.js", "2.3.2.1"},
                {"Splat", "1.6.2"},
                {"Sprache", "2.0.0.52"},
                {"sqlite-net-pcl", "1.1.2"},
                {"structuremap", "4.2.0.402"},
                {"WebGrease", "1.6.0"},
                {"WebMarkupMin.Core", "2.1.0"},
                {"Wire", "0.7.1"},
                {"xunit", "2.2.0-beta2-build3300"},
                {"Zlib.Portable", "1.11.0"},                
                #endregion
            };

            var result = theStack.AsParallel()
                .WithDegreeOfParallelism(3)
                .Select(i => new
                {
                    Name = i.Key,
                    Version = i.Value,
                    SupportType = GetPackage(i.Key, i.Value).SupportType
                })
                .OrderBy(i => i.Name)
                .Select(i => i.Name.PadRight(52) + i.Version.PadRight(25) + i.SupportType)
                .ToArray();

            var str = string.Join(Environment.NewLine, result);
            Approvals.Verify(str);
        }
    }
}