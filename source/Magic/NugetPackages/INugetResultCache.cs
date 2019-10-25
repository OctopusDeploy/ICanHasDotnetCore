using System.Threading;
using System.Threading.Tasks;
using ICanHasDotnetCore.Plumbing;
using NuGet.Packaging.Core;

namespace ICanHasDotnetCore.NugetPackages
{
    public interface INugetResultCache
    {
        Task<Option<NugetPackage>> GetAsync(PackageIdentity identity, CancellationToken cancellationToken);
        Task StoreAsync(NugetPackage package, CancellationToken cancellationToken);
    }

    public class NoNugetResultCache : INugetResultCache
    {
        public Task<Option<NugetPackage>> GetAsync(PackageIdentity identity, CancellationToken cancellationToken)
        {
            return Task.FromResult(Option<NugetPackage>.ToNone);
        }

        public Task StoreAsync(NugetPackage package, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}