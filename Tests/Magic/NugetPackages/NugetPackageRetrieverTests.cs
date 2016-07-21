using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using ICanHasDotnetCore.NugetPackages;
using NuGet;
using NUnit.Framework;

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
            yield return CreateTestCase("Non .NET library", "JQuery", "3.1.0", SupportType.NonDotNet);
            yield return CreateTestCase("Package doesn't list supported frameworks but is a .NET lib", "Antlr", "3.5.0.2", SupportType.Unsupported);
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
            var package = new NugetPackageInfoRetriever(new PackageRepositoryWrapper())
                 .Retrieve(id, new SemanticVersion(version))
                 .Result;
            if (package.WasFailure)
                Assert.Fail(package.ErrorString);
            return package;
        }


        [Test]
        public void NonExistantPackageShouldBeNotFound()
        {
            var pkg = GetPackage("FooFooFoo", "1.0.23523");
            pkg.SupportType.Should().Be(SupportType.NotFound);
        }
    }
}