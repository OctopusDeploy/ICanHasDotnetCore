using System;
using System.IO;
using System.Threading.Tasks;
using NuGet;
using Serilog;

namespace ICanHasDotnetCore.Magic
{
    public interface IPackageRepositoryWrapper
    {
        Task<IPackage> GetLatestPackage(string id);
        Task<Result<byte[]>> DownloadPackage(IPackageName package);
    }

    public class PackageRepositoryWrapper : IPackageRepositoryWrapper
    {
        private readonly DataServicePackageRepository _repository = new DataServicePackageRepository(new Uri("https://www.nuget.org/api/v2"));
        public Task<IPackage> GetLatestPackage(string id)
        {
            return Task.Run(() => _repository.FindPackage(id));
        }

        public Task<Result<byte[]>> DownloadPackage(IPackageName package)
        {
            return Task.Run(() => DownloadPackageTask(package));
        }

        private Result<byte[]> DownloadPackageTask(IPackageName package)
        {
            var dataServicePackage = package as DataServicePackage;
            if (dataServicePackage == null)
            {
                Log.Information("Package {id} is an unrecognised type: {type}", package.Id, package.GetType());
                return Result<byte[]>.Failed($"Error processing package {package.Id}");
            }

            using (var ms = new MemoryStream())
            {
                _repository.PackageDownloader.DownloadPackage(dataServicePackage.DownloadUrl, dataServicePackage, ms);
                return ms.ToArray();
            }
        }
    }
}