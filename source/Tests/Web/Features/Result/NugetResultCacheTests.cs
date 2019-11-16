using System;
using System.Data.Common;
using System.Runtime.Versioning;
using FluentAssertions;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Web.Database;
using ICanHasDotnetCore.Web.Features.result.Cache;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Xunit;

namespace ICanHasDotnetCore.Tests.Web.Features.Result
{
    public class NugetResultCacheTests : IDisposable
    {
        private readonly INugetResultCache _cache;
        private readonly DbConnection _connection;

        public NugetResultCacheTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;
            using (var context = new AppDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            _cache = new DbNugetResultCache(() => new AppDbContext(options));
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        [Fact]
        void NugetResultCache_ValidPackage_StoresAndRetrieves()
        {
            // Arrange
            const string id = "id";
            var dependencies = new[] {"dep1", "dep2", "dep3"};
            var version = new NuGetVersion(1, 2, 3);
            var frameworks = new[] {new FrameworkName(".NETFramework,Version=v4.0"), new FrameworkName(".NETFramework,Version=v4.5")};
            var expectedPackage = new NugetPackage(id, dependencies, SupportType.Supported, version, frameworks);

            // Act
            _cache.Store(expectedPackage);
            var package = _cache.Get(new PackageIdentity(id, version));

            // Assert
            package.Some.Should().BeTrue();
            package.Value.Should().BeEquivalentTo(expectedPackage);
        }

        [Fact]
        void NugetResultCache_NotInCache_ReturnsNone()
        {
            // Act
            var package = _cache.Get(new PackageIdentity("none", new NuGetVersion(1, 2, 3)));

            // Assert
            package.None.Should().BeTrue();
        }
    }
}