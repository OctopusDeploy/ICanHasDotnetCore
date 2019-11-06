using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace ICanHasDotnetCore.NugetPackages
{
    public class PackageRepositoryWrapper : IPackageRepositoryWrapper
    {
        private readonly SourceRepository _sourceRepository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

        private readonly SourceCacheContext _sourceCacheContext = new SourceCacheContext();
        private readonly ILogger _logger;

        private PackageMetadataResource _packageMetadataResource;
        private DownloadResource _downloadResource;

        public PackageRepositoryWrapper(ILogger logger)
        {
            _logger = logger ?? NullLogger.Instance;
        }

        private async Task<PackageMetadataResource> PackageMetadataResourceAsync(CancellationToken cancellationToken)
        {
            return _packageMetadataResource ??= await _sourceRepository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
        }

        private async Task<DownloadResource> DownloadResourceAsync(CancellationToken cancellationToken)
        {
            return _downloadResource ??= await _sourceRepository.GetResourceAsync<DownloadResource>(cancellationToken);
        }

        private async Task<PackageReaderBase> GetPackageReaderAsync(PackageIdentity identity, CancellationToken cancellationToken)
        {
            var downloadResource = await DownloadResourceAsync(cancellationToken);
            var globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(NullSettings.Instance);
            var result = await downloadResource.GetDownloadResourceResultAsync(identity, new PackageDownloadContext(_sourceCacheContext), globalPackagesFolder, _logger, cancellationToken);
            return result.PackageReader;
        }

        public async Task<IPackage> GetLatestPackageAsync(string id, bool prerelease, CancellationToken cancellationToken)
        {
            var packageMetadataResource = await PackageMetadataResourceAsync(cancellationToken);
            const bool includeUnlisted = false;
            var allVersionsMetadata = await packageMetadataResource.GetMetadataAsync(id, prerelease, includeUnlisted, _sourceCacheContext, _logger, cancellationToken);
            var latestVersionMetadata = allVersionsMetadata.OrderByDescending(e => e.Identity.Version).FirstOrDefault();
            if (latestVersionMetadata == null)
                return null;
            return await GetPackageAsync(id, latestVersionMetadata.Identity.Version, cancellationToken);
        }

        public async Task<IPackage> GetPackageAsync(string id, NuGetVersion version, CancellationToken cancellationToken)
        {
            var packageMetadataResource = await PackageMetadataResourceAsync(cancellationToken);
            var metadata = await packageMetadataResource.GetMetadataAsync(new PackageIdentity(id, version), _sourceCacheContext, _logger, cancellationToken);
            return metadata == null || !metadata.IsListed ? null : new Package(metadata, GetPackageReaderAsync);
        }
    }
}