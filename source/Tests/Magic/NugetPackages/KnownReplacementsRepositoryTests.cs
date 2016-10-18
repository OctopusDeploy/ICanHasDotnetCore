using FluentAssertions;
using ICanHasDotnetCore.NugetPackages;
using NUnit.Framework;

namespace ICanHasDotnetCore.Tests.Magic.NugetPackages
{
    public class KnownReplacementsRepositoryTests
    {
        [Test]
        public void EntriesCanBeReadAndAtLeastOneEntryExists()
        {
            KnownReplacementsRepository.All.Should().NotBeEmpty();
        }
    }
}