using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Plumbing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NuGet;

namespace ICanHasDotnetCore.Web.Database
{
    public class NugetPackageConfiguration : IEntityTypeConfiguration<NugetPackage>
    {
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
            var versionConverter = new ValueConverter<Option<SemanticVersion>, string>
            (
                option => option.Value.ToNormalizedString(),
                value => SemanticVersion.Parse(value)
            );
            builder.HasKey(e => new {e.Id, e.Version});
            builder.Property(e => e.Version).HasConversion(versionConverter);
            builder.Property(e => e.SupportType).HasConversion<string>();
            builder.Property(e => e.ProjectUrl);
            builder.Property(e => e.Dependencies).HasConversion(dependenciesConverter);
            builder.Property(e => e.Frameworks).HasConversion(frameworksConverter);
        }
    }
}