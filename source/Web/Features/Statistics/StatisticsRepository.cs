using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Web.Database;
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

        public async Task AddStatisticsForResult(InvestigationResult result)
        {
            try
            {
                var tasks = result.GetAllDistinctRecursive()
                    .Where(p => AddStatisticsFor.Contains(p.SupportType))
                    .Select(AddStatistic);

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception writing statistics");
            }
        }

        private async Task AddStatistic(PackageResult package)
        {
            using (var context = _contextFactory())
            {
                var packageStatistic = await context.PackageStatistics.FindAsync(package.PackageName);
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
                await context.SaveChangesAsync();
            }
        }

        public IReadOnlyList<PackageStatistic> GetAllPackageStatistics()
        {
            using (var context = _contextFactory())
            {
                return context.PackageStatistics.ToList();
            }
        }

        public void UpdateSupportTypeFor(PackageStatistic stat, SupportType supportType)
        {
            using (var context = _contextFactory())
            {
                var packageStatistic = context.PackageStatistics.Find(stat.Name);
                if (packageStatistic != null)
                {
                    packageStatistic.LatestSupportType = supportType;
                    context.SaveChanges();
                }
            }
        }
    }
}