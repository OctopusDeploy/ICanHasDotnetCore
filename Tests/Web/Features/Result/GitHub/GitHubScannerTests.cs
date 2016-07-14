using System.Threading.Tasks;
using FluentAssertions;
using ICanHasDotnetCore.Web.Features.Result.GitHub;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Serilog;
using Tests.DNC.Web.Helpers;

namespace ICanHasDotnetCore.Tests.Web.Features.Result.GitHub
{
    public class GitHubScannerTests
    {

        [Test]
        public async Task ICanHasDotnetRepository()
        {
            var result = await CreateScanner().Scan("/OctopusDeploy/ICanHasDotnetCore\\");
            result.WasSuccessful.Should().BeTrue(result.ErrorString);
            var names = result.Value.Select(p => p.Name).ToArray();
            names.ShouldAllBeEquivalentTo(new[] { "packages.config"});
        }

        [Test]
        public async Task InvalidId_SingleSegment()
        {
            var result = await CreateScanner().Scan("OctopusDeploy");
            result.WasSuccessful.Should().BeFalse();
            result.ErrorString.Should().Be("OctopusDeploy is not recognised as a GitHub repository name");
        }

        [Test]
        public async Task InvalidId_ExtraSegments()
        {
            var result = await CreateScanner().Scan("OctopusDeploy/Foo/Bar");
            result.WasSuccessful.Should().BeFalse();
            result.ErrorString.Should().Be("OctopusDeploy/Foo/Bar is not recognised as a GitHub repository name");
        }

        [Test]
        public async Task RepoDoesNotExist()
        {
            var result = await CreateScanner().Scan("OctopusDeploy/DoesNotExist");
            result.WasSuccessful.Should().BeFalse();
            result.ErrorString.Should().Be("OctopusDeploy/DoesNotExist does not exist or is not publically accessible");
        }

   

        private GitHubScanner CreateScanner()
        {
            return new GitHubScanner(
                new FakeConfigurationRoot()
                {
                    {"GitHubToken", null }
                }
            );
        }
    }
}