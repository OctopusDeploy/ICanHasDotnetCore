using FluentAssertions;
using ICanHasDotnetCore.NugetPackages;
using Xunit;

namespace ICanHasDotnetCore.Tests
{
    public class NugetPackageRetrieverTests
    {
        [Fact]
        public void DependenciesCanBeParsedForSerilogSinkSeq()
        {
            var retriever = GetRetriever();
            var pkg = retriever.Retrieve("Serilog.Sinks.Seq", true).Result;
            pkg.Id.Should().Be("Serilog.Sinks.Seq");
            pkg.Dependencies.Should().Contain("Serilog.Sinks.RollingFile", "Serilog", "Serilog.Sinks.PeriodicBatching");
        }

        [Fact]
        public void DependenciesCanBeParsedForAutofac()
        {
            var retriever = GetRetriever();
            var pkg = retriever.Retrieve("Autofac", true).Result;
            pkg.Id.Should().Be("Autofac");
            pkg.Dependencies.Should().BeEquivalentTo();
        }

        private NugetPackageInfoRetriever GetRetriever()
        {
            return new NugetPackageInfoRetriever(new PackageRepositoryWrapper());
        }

        [Fact]
        public void CompatiblePackageCanBeIdentified()
        {
            var retriever = GetRetriever();
            var pkg = retriever.Retrieve("Serilog.Sinks.Seq", false).Result;
            pkg.SupportType.Should().Be(SupportType.Supported);
        }

        [Fact]
        public void CompatiblePrereleasePackageCanBeIdentified()
        {
            var retriever = GetRetriever();
            var pkg = retriever.Retrieve("Serilog.Sinks.Seq", true).Result;
            pkg.SupportType.Should().Be(SupportType.PreRelease);
        }

        [Fact]
        public void IncompatiblePackageCanBeIdentified()
        {
            var retriever = GetRetriever();
            var pkg = retriever.Retrieve("BootstrapMvcHelpers", false).Result;
            pkg.SupportType.Should().Be(SupportType.Unsupported);
        }

        [Fact]
        public void NonExistantPackageShouldBeNotFound()
        {
            var retriever = GetRetriever();
            var pkg = retriever.Retrieve("FooFooFoo", true).Result;
            pkg.SupportType.Should().Be(SupportType.NotFound);
        }
    }
}