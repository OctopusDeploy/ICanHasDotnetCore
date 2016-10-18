using FluentAssertions;
using ICanHasDotnetCore.NugetPackages;
using NUnit.Framework;

namespace ICanHasDotnetCore.Tests.Magic.NugetPackages
{
    public class MoreInformationRepositoryTests
    {
        [Test]
        public void EntriesCanBeReadAndAtLeastOneEntryExists()
        {
            MoreInformationRepository.All.Should().NotBeEmpty();
        }
    }
}