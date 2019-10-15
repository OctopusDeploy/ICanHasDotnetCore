using System.Collections.Generic;
using System.Threading.Tasks;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Web.Features.Statistics;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace ICanHasDotnetCore.Tests.Web.Features.Statistics
{
    public class RequerySupportTypeForStatisticsPackagesTaskTests
    {

        [Fact]
        public void TestRequery()
        {
            var repo = new TestRepository();
            var task = new RequerySupportTypeForStatisticsPackagesTask(repo);
            task.Run().Wait();
            repo.Updates.Keys.ShouldAllBeEquivalentTo(TestRepository.PackageNames);
            repo.Updates["JQuery"].Should().Be(SupportType.NoDotNetLibraries, "JQuery is a no dotnet result");
            repo.Updates["Xunit.Core"].Should().Be(SupportType.Supported, "Forwarding packages are Supported");
        }

        private class TestRepository : IStatisticsRepository
        {
            public readonly Dictionary<string, SupportType> Updates = new Dictionary<string, SupportType>();

            public static readonly string[] PackageNames = { "Autofac", "JQuery", "Xunit.Core" };

            public Task AddStatisticsForResult(InvestigationResult result)
            {
                throw new System.NotImplementedException();
            }

            public IReadOnlyList<PackageStatistic> GetAllPackageStatistics()
            {
                return PackageNames
                    .Select(n => new PackageStatistic() { Name = n })
                    .ToArray();
            }

            public void UpdateSupportTypeFor(PackageStatistic stat, SupportType supportType)
            {
                Updates[stat.Name] = supportType;
            }
        }
    }
}