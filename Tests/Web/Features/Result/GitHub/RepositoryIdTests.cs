using FluentAssertions;
using ICanHasDotnetCore.Web.Features.Result.GitHub;
using NUnit.Framework;

namespace ICanHasDotnetCore.Tests.Web.Features.Result.GitHub
{
    public class RepositoryIdTests
    {
        [TestCase("OctopusDeploy/Foo")]
        [TestCase("/OctopusDeploy/Foo/")]
        [TestCase("/OctopusDeploy/Foo")]
        [TestCase("/Octopus_Deploy/Fo_o/")]
        [TestCase("/Octopus-Deploy/Fo-o/")]
        [TestCase("/Octopus.Deploy/Fo.o/")]
        [TestCase("Oct_opus.Depl-oy/F_o.o-")]
        public void ValidNames(string name)
        {
            GitHubScanner.RepositoryId.Parse(name)
                .Some.Should().BeTrue();
        }

        [TestCase("OctopusDeploy/Foo/Bar")]
        [TestCase("Octopus$Deploy/Foo")]
        [TestCase("Octopus\\Deploy/Foo")]
        [TestCase("Octopus=Deploy/Foo")]
        [TestCase("OctopusDeploy/F\\oo")]
        [TestCase("Octo:pusDeploy/Foo")]
        [TestCase("OctopusDeploy/F:oo")]
        public void InvalidNames(string name)
        {
            GitHubScanner.RepositoryId.Parse(name)
                .None.Should().BeTrue();
        }
    }
}