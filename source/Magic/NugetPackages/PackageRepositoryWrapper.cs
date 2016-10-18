using System;
using System.Threading.Tasks;
using NuGet;

namespace ICanHasDotnetCore.NugetPackages
{
    public interface IPackageRepositoryWrapper
    {
        Task<IPackage> GetLatestPackage(string id, bool prerelease);
        Task<IPackage> GetPackage(string id, SemanticVersion version);
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
    }
}