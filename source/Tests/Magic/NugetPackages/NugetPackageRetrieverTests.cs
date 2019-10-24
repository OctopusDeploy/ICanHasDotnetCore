using System.Collections.Generic;
using FluentAssertions;
using ICanHasDotnetCore.NugetPackages;
using NuGet;
using Xunit;

namespace ICanHasDotnetCore.Tests.Magic.NugetPackages
{
    public class NugetPackageRetrieverTests
    {
        public static IEnumerable<object[]> TestCases()
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


        public static object[] CreateTestCase(string name, string id, string version, SupportType expectedSupportType, params string[] expectedDependencies)
            => new object[] {name, id, version, expectedSupportType, expectedDependencies};


        [Theory]
        [MemberData(nameof(TestCases))]
        public void PackageIsRetrieved(string name, string id, string version, SupportType expectedSupportType, string[] expectedDependencies)
        {
            GetPackage(id, version).Id.Should().Be(id);
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void PackageSupportTypeIsCorrect(string name, string id, string version, SupportType expectedSupportType, string[] expectedDependencies)
        {
            GetPackage(id, version).SupportType.Should().Be(expectedSupportType);
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void PackageDependenciesHaveBeenExtractedCorrectly(string name, string id, string version, SupportType expectedSupportType, string[] expectedDependencies)
        {
            GetPackage(id, version).Dependencies.Should().BeEquivalentTo(expectedDependencies);
        }

        private NugetPackage GetPackage(string id, string version)
        {
            return new NugetPackageInfoRetriever(new PackageRepositoryWrapper(), new NoNugetResultCache())
                 .Retrieve(id, new SemanticVersion(version))
                 .Result;
        }


        [Fact]
        public void NonExistantPackageShouldBeNotFound()
        {
            var pkg = GetPackage("FooFooFoo", "1.0.23523");
            pkg.SupportType.Should().Be(SupportType.NotFound);
        }
    }
}