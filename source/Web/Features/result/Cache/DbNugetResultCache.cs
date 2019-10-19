using System;
using System.Linq;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.Web.Database;
using NuGet;
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

        public Option<NugetPackage> Get(string id, SemanticVersion version)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var package = context.NugetResultCache.SingleOrDefault(e => e.Id == id && e.Version.Value == version);
                    return package != null ? Option<NugetPackage>.ToSome(package) : Option<NugetPackage>.ToNone;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Could not retrieve {id} {version} in Nuget Package Cache", id, version);
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
