using System;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.Web.Database;
using NuGet.Packaging.Core;
using Serilog;

namespace ICanHasDotnetCore.Web.Features.result.Cache
{
    public class DbNugetResultCache : INugetResultCache
    {
        private readonly Func<AppDbContext> _contextFactory;

        public DbNugetResultCache(Func<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Option<NugetPackage> Get(PackageIdentity identity)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    // Finding a entity with a value converter on its primary key is tricky, see https://github.com/aspnet/EntityFrameworkCore/issues/14180
                    var package = context.NugetResultCache.Find(identity.Id, identity.Version.Some());
                    return package?.Some() ?? package.None();
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Could not retrieve {id} {version} in Nuget Package Cache", identity.Id, identity.Version);
                return Option<NugetPackage>.ToNone;
            }
        }

        public void Store(NugetPackage package)
        {
            try
            {
                if (package.Version.None)
                    return;

                using (var context = _contextFactory())
                {
                    context.NugetResultCache.Add(package);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Could not store {id} {version} in Nuget Package Cache", package.Id, package.Version.Value);
            }
        }
    }
}
