using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using ICanHasDotnetCore.Plumbing;
using NuGet.Versioning;

namespace ICanHasDotnetCore.NugetPackages
{

    public class NugetPackageInfoRetriever
    {
        private readonly IPackageRepositoryWrapper _repository;
        private readonly INugetResultCache _nugetResultCache;

        public NugetPackageInfoRetriever(IPackageRepositoryWrapper repository, INugetResultCache nugetResultCache)
        {
            _repository = repository;
            _nugetResultCache = nugetResultCache;
        }

        public async Task<NugetPackage> RetrieveAsync(string id, bool includePrerelease, CancellationToken cancellationToken)
        {
            var package = await _repository.GetLatestPackageAsync(id, includePrerelease);
            return await RetrieveAsync(id, package, cancellationToken);
        }

        public async Task<NugetPackage> RetrieveAsync(string id, NuGetVersion version, CancellationToken cancellationToken)
        {
            var package = await _repository.GetPackageAsync(id, version);
            return await RetrieveAsync(id, package, cancellationToken);
        }

        private async Task<NugetPackage> RetrieveAsync(string id, IPackage package, CancellationToken cancellationToken)
        {
            if (package == null)
                return new NugetPackage(id, new string[0], SupportType.NotFound, Option<NuGetVersion>.ToNone, new FrameworkName[0]);

            var cached = await _nugetResultCache.GetAsync(package.Identity, cancellationToken);
            if (cached.Some)
                return cached.Value;

            var result = await package.GetNugetPackageAsync(CancellationToken.None);
            await _nugetResultCache.StoreAsync(result, cancellationToken);

            return result;
        }
    }
}