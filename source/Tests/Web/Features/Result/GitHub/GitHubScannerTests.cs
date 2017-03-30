using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;
using ICanHasDotnetCore.Tests.Web.Helpers;
using ICanHasDotnetCore.Web.Features.result.GitHub;
using Microsoft.Extensions.Configuration;
using Xunit;
using Serilog;

namespace ICanHasDotnetCore.Tests.Web.Features.Result.GitHub
{
    public class GitHubScannerTests
    {

        [Fact]
        public async Task ICanHasDotnetRepository()
        {
            var result = await CreateScanner().Scan("/OctopusDeploy/ICanHasDotnetCore\\");
            result.WasSuccessful.Should().BeTrue(result.ErrorString);
            var names = result.Value.Select(p => p.Name).ToArray();
            names.ShouldAllBeEquivalentTo(new[] { "source/Magic", "source/Database", "source/Console", "source/Tests", "source/Web" });
        }

        
        [Fact]
        public async Task InvalidId_InvalidName()
        {
            var result = await CreateScanner().Scan("OctopusDeploy/Foo/Bar");
            result.WasSuccessful.Should().BeFalse();
            result.ErrorString.Should().Be("OctopusDeploy/Foo/Bar is not recognised as a GitHub repository name");
        }

        [Fact]
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