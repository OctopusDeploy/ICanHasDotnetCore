using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Web.Features.Statistics;
using Xunit;
using FluentAssertions;
using System.Linq;
using System.Threading;

namespace ICanHasDotnetCore.Tests.Web.Features.Statistics
{
    [SuppressMessage("ReSharper", "VSTHRD200")]
    public class RequerySupportTypeForStatisticsPackagesTaskTests
    {

        [Fact]
        public async Task TestRequery()
        {
            var repo = new TestRepository();
            var task = new RequerySupportTypeForStatisticsPackagesTask(repo);
            await task.RunAsync(CancellationToken.None);
            repo.Updates.Keys.Should().BeEquivalentTo(TestRepository.PackageNames);
            repo.Updates["JQuery"].Should().Be(SupportType.NoDotNetLibraries, "JQuery is a no dotnet result");
            repo.Updates["Xunit.Core"].Should().Be(SupportType.Supported, "Forwarding packages are Supported");
        }

        private class TestRepository : IStatisticsRepository
        {
            public readonly Dictionary<string, SupportType> Updates = new Dictionary<string, SupportType>();

            public static readonly string[] PackageNames = { "Autofac", "JQuery", "Xunit.Core" };

            public Task AddStatisticsForResultAsync(InvestigationResult result, CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }

            public Task<IReadOnlyList<PackageStatistic>> GetAllPackageStatisticsAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult<IReadOnlyList<PackageStatistic>>(PackageNames
                    .Select(n => new PackageStatistic() { Name = n })
                    .ToArray());
            }

            public Task UpdateSupportTypeAsync(PackageStatistic stat, SupportType supportType, CancellationToken cancellationToken)
            {
                Updates[stat.Name] = supportType;
                return Task.CompletedTask;
            }
        }
    }
}