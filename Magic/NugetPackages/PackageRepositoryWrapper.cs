using System;
using System.IO;
using System.Threading.Tasks;
using ICanHasDotnetCore.Plumbing;
using NuGet;
using Serilog;

namespace ICanHasDotnetCore.NugetPackages
{
    public interface IPackageRepositoryWrapper
    {
        Task<IPackage> GetLatestPackage(string id, bool prerelease);
        Task<IPackage> GetPackage(string id, SemanticVersion version);
        Task<Result<ZipPackage>> DownloadPackage(IPackageName package);
    }

    public class PackageRepositoryWrapper : IPackageRepositoryWrapper
    {
        private readonly DataServicePackageRepository _repository = new DataServicePackageRepository(new Uri("https://www.nuget.org/api/v2"));
        public Task<IPackage> GetLatestPackage(string id, bool prerelease)
        {
            return Task.Run(() => _repository.FindPackage(id, (IVersionSpec)null, prerelease, false));
        }

        public Task<IPackage> GetPackage(string id, SemanticVersion version)
        {
            return Task.Run(() => _repository.FindPackage(id, new VersionSpec(version), true, false));
        }

        public Task<Result<ZipPackage>> DownloadPackage(IPackageName package)
        {
            return Task.Run(() => DownloadPackageTask(package));
        }

        private Result<ZipPackage> DownloadPackageTask(IPackageName package)
        {
            var dataServicePackage = package as DataServicePackage;
            if (dataServicePackage == null)
            {
                Log.Information("Package {id} is an unrecognised type: {type}", package.Id, package.GetType());
                return Result<ZipPackage>.Failed($"Error processing package {package.Id}");
            }

            var ms = new MemoryStream();
            _repository.PackageDownloader.DownloadPackage(dataServicePackage.DownloadUrl, dataServicePackage, ms);
            ms.Position = 0;
            return new ZipPackage(ms);
        }
    }
}