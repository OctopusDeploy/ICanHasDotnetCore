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

        public async Task<NugetPackage> Retrieve(string id, bool includePrerelease)
        {
            var package = await _repository.GetLatestPackage(id, includePrerelease);
            return await Retrieve(id, package);
        }

        public async Task<NugetPackage> Retrieve(string id, NuGetVersion version)
        {
            var package = await _repository.GetPackage(id, version);
            return await Retrieve(id, package);
        }
        
        private async Task<NugetPackage> Retrieve(string id, IPackage package)
        {
            if (package == null)
                return new NugetPackage(id, new string[0], SupportType.NotFound, Option<NuGetVersion>.ToNone, new FrameworkName[0]);

            var cached = _nugetResultCache.Get(package.Identity);
            if (cached.Some)
                return cached.Value;

            var result = await package.GetNugetPackageAsync(CancellationToken.None);
            _nugetResultCache.Store(result);

            return result;
        }
    }
}