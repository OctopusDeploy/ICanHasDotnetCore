using FluentAssertions;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.Web.Features.result.GitHub;
using Xunit;

namespace ICanHasDotnetCore.Tests.Web.Features.Result.GitHub
{
    public class RepositoryReferenceTests
    {
        [Theory]
        [InlineData("OctopusDeploy/Foo", false)]
        [InlineData("OctopusDeploy/Foo@sha", true)]
        [InlineData("/OctopusDeploy/Foo/", false)]
        [InlineData("/OctopusDeploy/Foo/@sha", true)]
        [InlineData("/OctopusDeploy/Foo", false)]
        [InlineData("/OctopusDeploy/Foo@sha", true)]
        [InlineData("/Octopus_Deploy/Fo_o/", false)]
        [InlineData("/Octopus_Deploy/Fo_o/@sha", true)]
        [InlineData("/Octopus-Deploy/Fo-o/", false)]
        [InlineData("/Octopus-Deploy/Fo-o/@sha", true)]
        [InlineData("/Octopus.Deploy/Fo.o/", false)]
        [InlineData("/Octopus.Deploy/Fo.o/@sha", true)]
        [InlineData("Oct_opus.Depl-oy/F_o.o-", false)]
        [InlineData("Oct_opus.Depl-oy/F_o.o-@sha", true)]
        public void ValidNames(string name, bool hasReference)
        {
            var result = GitHubScanner.RepositoryReference.Parse(name);
            result.Some.Should().BeTrue();
            result.Value.Reference.Some.Should().Be(hasReference);
            if (result.Value.Reference.Some)
                result.Value.Reference.Value.Should().Be("sha");
        }

        [Theory]
        [InlineData("OctopusDeploy/Foo/Bar")]
        [InlineData("OctopusDeploy/Foo/Bar@sha")]
        [InlineData("Octopus$Deploy/Foo")]
        [InlineData("Octopus$Deploy/Foo@sha")]
        [InlineData("Octopus\\Deploy/Foo")]
        [InlineData("Octopus\\Deploy/Foo@sha")]
        [InlineData("Octopus=Deploy/Foo")]
        [InlineData("Octopus=Deploy/Foo@sha")]
        [InlineData("OctopusDeploy/F\\oo")]
        [InlineData("OctopusDeploy/F\\oo@sha")]
        [InlineData("Octo:pusDeploy/Foo")]
        [InlineData("Octo:pusDeploy/Foo@sha")]
        [InlineData("OctopusDeploy/F:oo")]
        [InlineData("OctopusDeploy/F:oo@sha")]
        public void InvalidNames(string name)
        {
            GitHubScanner.RepositoryReference.Parse(name)
                .None.Should().BeTrue();
        }
    }
}