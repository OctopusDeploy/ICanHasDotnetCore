using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Web.Features.Statistics;
using Microsoft.EntityFrameworkCore;

namespace ICanHasDotnetCore.Web.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new NugetPackageConfiguration());
            modelBuilder.ApplyConfiguration(new PackageStatisticConfiguration());
        }

        public DbSet<NugetPackage> NugetResultCache { get; set; }
        public DbSet<PackageStatistic> PackageStatistics { get; set; }
    }
}