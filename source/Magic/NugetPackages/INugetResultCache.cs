using ICanHasDotnetCore.Plumbing;
using NuGet;

namespace ICanHasDotnetCore.NugetPackages
{
    public interface INugetResultCache
    {
        Option<NugetPackage> Get(string id, SemanticVersion version);
        void Store(NugetPackage package);
    }

    public class NoNugetResultCache : INugetResultCache
    {
        public Option<NugetPackage> Get(string id, SemanticVersion version)
        {
            return Option<NugetPackage>.ToNone;
        }

        public void Store(NugetPackage package)
        {
        }
    }
}