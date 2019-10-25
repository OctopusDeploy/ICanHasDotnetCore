using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
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
    [SuppressMessage("ReSharper", "VSTHRD200")]
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
        async Task NugetResultCache_ValidPackage_StoresAndRetrieves()
        {
            // Arrange
            const string id = "id";
            var dependencies = new[] {"dep1", "dep2", "dep3"};
            var version = new NuGetVersion(1, 2, 3);
            var frameworks = new[] {new FrameworkName(".NETFramework,Version=v4.0"), new FrameworkName(".NETFramework,Version=v4.5")};
            var expectedPackage = new NugetPackage(id, dependencies, SupportType.Supported, version, frameworks);

            // Act
            await _cache.StoreAsync(expectedPackage, CancellationToken.None);
            var package = await _cache.GetAsync(new PackageIdentity(id, version), CancellationToken.None);

            // Assert
            package.Some.Should().BeTrue();
            package.Value.Should().BeEquivalentTo(expectedPackage);
        }

        [Fact]
        async Task NugetResultCache_NotInCache_ReturnsNone()
        {
            // Act
            var package = await _cache.GetAsync(new PackageIdentity("none", new NuGetVersion(1, 2, 3)), CancellationToken.None);

            // Assert
            package.None.Should().BeTrue();
        }
    }
}