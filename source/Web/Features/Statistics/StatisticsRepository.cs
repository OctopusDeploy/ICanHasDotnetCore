using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Web.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ICanHasDotnetCore.Web.Features.Statistics
{
    public class StatisticsRepository : IStatisticsRepository
    {
        // Only log packages found on Nuget.org
        private static readonly SupportType[] AddStatisticsFor = { SupportType.Unsupported, SupportType.Supported, SupportType.PreRelease };

        private readonly Func<AppDbContext> _contextFactory;

        public StatisticsRepository(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddStatisticsForResultAsync(InvestigationResult result, CancellationToken cancellationToken)
        {
            try
            {
                var tasks = result.GetAllDistinctRecursive()
                    .Where(p => AddStatisticsFor.Contains(p.SupportType))
                    .Select(p => AddStatisticAsync(p, cancellationToken));

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception writing statistics");
            }
        }

        private async Task AddStatisticAsync(PackageResult package, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            var packageStatistic = await context.PackageStatistics.FindAsync(new object[] {package.PackageName}, cancellationToken);
            if (packageStatistic == null)
            {
                context.PackageStatistics.Add(new PackageStatistic
                {
                    Name = package.PackageName,
                    Count = 1,
                    LatestSupportType = package.SupportType
                });
            }
            else
            {
                packageStatistic.Count += 1;
                packageStatistic.LatestSupportType = package.SupportType;
            }
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<PackageStatistic>> GetAllPackageStatisticsAsync(CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            return await context.PackageStatistics.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task UpdateSupportTypeAsync(PackageStatistic stat, SupportType supportType, CancellationToken cancellationToken)
        {
            await using var context = _contextFactory();
            var packageStatistic = await context.PackageStatistics.FindAsync(new object[] {stat.Name}, cancellationToken);
            if (packageStatistic != null)
            {
                packageStatistic.LatestSupportType = supportType;
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}