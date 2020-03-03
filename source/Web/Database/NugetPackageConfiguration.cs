using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Plumbing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NuGet.Versioning;

namespace ICanHasDotnetCore.Web.Database
{
    public class NugetPackageConfiguration : IEntityTypeConfiguration<NugetPackage>
    {
        private static ValueComparer<IReadOnlyList<T>> CreateIReadOnlyListComparer<T>()
        {
            // Adapted from https://github.com/dotnet/efcore/issues/17471#issuecomment-526330450
            return new ValueComparer<IReadOnlyList<T>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())));
        }

        public void Configure(EntityTypeBuilder<NugetPackage> builder)
        {
            var dependenciesConverter = new ValueConverter<IReadOnlyList<string>, string>
            (
                list => string.Join("|", list),
                value => value.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
            );
            var frameworksConverter = new ValueConverter<IReadOnlyList<FrameworkName>, string>
            (
                list => string.Join("|", list),
                value => value.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries).Select(e => new FrameworkName(e)).ToArray()
            );
            var versionConverter = new ValueConverter<Option<NuGetVersion>, string>
            (
                option => option.Value.ToNormalizedString(),
                value => NuGetVersion.Parse(value)
            );
            builder.HasKey(e => new {e.Id, e.Version});
            builder.Property(e => e.Version).HasConversion(versionConverter);
            builder.Property(e => e.SupportType).HasConversion<string>();
            builder.Property(e => e.ProjectUrl);
            builder.Property(e => e.Dependencies).HasConversion(dependenciesConverter)
                .Metadata.SetValueComparer(CreateIReadOnlyListComparer<string>());
            builder.Property(e => e.Frameworks).HasConversion(frameworksConverter)
                .Metadata.SetValueComparer(CreateIReadOnlyListComparer<FrameworkName>());
        }
    }
}