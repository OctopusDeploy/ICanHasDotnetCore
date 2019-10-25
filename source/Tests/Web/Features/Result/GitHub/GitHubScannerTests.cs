using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;
using ICanHasDotnetCore.Web.Configuration;
using ICanHasDotnetCore.Web.Features.result.GitHub;
using Octokit;
using Xunit;

namespace ICanHasDotnetCore.Tests.Web.Features.Result.GitHub
{
    [SuppressMessage("ReSharper", "VSTHRD200")]
    public class GitHubScannerTests
    {

        [Fact()]
        public async Task ICanHasDotnetRepository()
        {
            var result = await CreateScanner().ScanAsync("/OctopusDeploy/ICanHasDotnetCore\\");
            result.WasSuccessful.Should().BeTrue(result.ErrorString);
            var names = result.Value.Select(p => p.Name).ToArray();
            names.Should().BeEquivalentTo(new[] { "source/Magic", "source/Database", "source/Console", "source/Tests", "source/Web" });
        }


        [Fact]
        public async Task InvalidId_InvalidName()
        {
            var result = await CreateScanner().ScanAsync("OctopusDeploy/Foo/Bar");
            result.WasSuccessful.Should().BeFalse();
            result.ErrorString.Should().Be("OctopusDeploy/Foo/Bar is not recognised as a GitHub repository name");
        }

        [Fact]
        public async Task RepoDoesNotExist()
        {
            var result = await CreateScanner().ScanAsync("OctopusDeploy/DoesNotExist");
            result.WasSuccessful.Should().BeFalse();
            result.ErrorString.Should().Be("OctopusDeploy/DoesNotExist does not exist or is not publically accessible");
        }



        private GitHubScanner CreateScanner()
        {
            return new GitHubScanner(new GitHubClient(new ProductHeaderValue(nameof(GitHubScannerTests))));
        }
    }
}