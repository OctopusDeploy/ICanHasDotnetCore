using ICanHasDotnetCore.Plumbing;
using NuGet.Packaging.Core;

namespace ICanHasDotnetCore.NugetPackages
{
    public interface INugetResultCache
    {
        Option<NugetPackage> Get(PackageIdentity identity);
        void Store(NugetPackage package);
    }

    public class NoNugetResultCache : INugetResultCache
    {
        public Option<NugetPackage> Get(PackageIdentity identity)
        {
            return Option<NugetPackage>.ToNone;
        }

        public void Store(NugetPackage package)
        {
        }
    }
}