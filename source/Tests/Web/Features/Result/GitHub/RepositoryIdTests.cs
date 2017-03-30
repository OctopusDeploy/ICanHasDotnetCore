using FluentAssertions;
using ICanHasDotnetCore.Web.Features.result.GitHub;
using Xunit;

namespace ICanHasDotnetCore.Tests.Web.Features.Result.GitHub
{
    public class RepositoryIdTests
    {
        [Theory]
        [InlineData("OctopusDeploy/Foo")]
        [InlineData("/OctopusDeploy/Foo/")]
        [InlineData("/OctopusDeploy/Foo")]
        [InlineData("/Octopus_Deploy/Fo_o/")]
        [InlineData("/Octopus-Deploy/Fo-o/")]
        [InlineData("/Octopus.Deploy/Fo.o/")]
        [InlineData("Oct_opus.Depl-oy/F_o.o-")]
        public void ValidNames(string name)
        {
            GitHubScanner.RepositoryId.Parse(name)
                .Some.Should().BeTrue();
        }

        [Theory]
        [InlineData("OctopusDeploy/Foo/Bar")]
        [InlineData("Octopus$Deploy/Foo")]
        [InlineData("Octopus\\Deploy/Foo")]
        [InlineData("Octopus=Deploy/Foo")]
        [InlineData("OctopusDeploy/F\\oo")]
        [InlineData("Octo:pusDeploy/Foo")]
        [InlineData("OctopusDeploy/F:oo")]
        public void InvalidNames(string name)
        {
            GitHubScanner.RepositoryId.Parse(name)
                .None.Should().BeTrue();
        }
    }
}