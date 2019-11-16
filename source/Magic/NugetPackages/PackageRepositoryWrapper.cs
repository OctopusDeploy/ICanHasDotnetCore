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
    public interface IPackageRepositoryWrapper
    {
        Task<IPackage> GetLatestPackage(string id, bool prerelease);
        Task<IPackage> GetPackage(string id, NuGetVersion version);
    }

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

        private async Task<PackageMetadataResource> PackageMetadataResource()
        {
            return _packageMetadataResource ?? (_packageMetadataResource = await _sourceRepository.GetResourceAsync<PackageMetadataResource>());
        }

        private async Task<DownloadResource> DownloadResource()
        {
            return _downloadResource ?? (_downloadResource = await _sourceRepository.GetResourceAsync<DownloadResource>());
        }

        private async Task<PackageReaderBase> GetPackageReaderAsync(PackageIdentity identity)
        {
            var downloadResource = await DownloadResource();
            var globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(NullSettings.Instance);
            var result = await downloadResource.GetDownloadResourceResultAsync(identity, new PackageDownloadContext(_sourceCacheContext), globalPackagesFolder, _logger, CancellationToken.None);
            return result.PackageReader;
        }

        public async Task<IPackage> GetLatestPackage(string id, bool prerelease)
        {
            var packageMetadataResource = await PackageMetadataResource();
            const bool includeUnlisted = false;
            var allVersionsMetadata = await packageMetadataResource.GetMetadataAsync(id, prerelease, includeUnlisted, _sourceCacheContext, _logger, CancellationToken.None);
            var latestVersionMetadata = allVersionsMetadata.OrderByDescending(e => e.Identity.Version).FirstOrDefault();
            if (latestVersionMetadata == null)
                return null;
            return await GetPackage(id, latestVersionMetadata.Identity.Version);
        }

        public async Task<IPackage> GetPackage(string id, NuGetVersion version)
        {
            var packageMetadataResource = await PackageMetadataResource();
            var metadata = await packageMetadataResource.GetMetadataAsync(new PackageIdentity(id, version), _sourceCacheContext, _logger, CancellationToken.None);
            return metadata == null || !metadata.IsListed ? null : new Package(metadata, GetPackageReaderAsync);
        }
    }
}