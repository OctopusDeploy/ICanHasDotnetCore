using ICanHasDotnetCore.Web.Features.Statistics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICanHasDotnetCore.Web.Database
{
    public class PackageStatisticConfiguration : IEntityTypeConfiguration<PackageStatistic>
    {
        public void Configure(EntityTypeBuilder<PackageStatistic> builder)
        {
            builder.HasKey(e => e.Name);
            builder.Property(e => e.Count);
            builder.Property(e => e.LatestSupportType).HasConversion<string>();
        }
    }
}