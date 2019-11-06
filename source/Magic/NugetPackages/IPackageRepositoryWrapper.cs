using System.Threading;
using System.Threading.Tasks;
using NuGet.Versioning;

namespace ICanHasDotnetCore.NugetPackages
{
    public interface IPackageRepositoryWrapper
    {
        Task<IPackage> GetLatestPackageAsync(string id, bool prerelease, CancellationToken cancellationToken);
        Task<IPackage> GetPackageAsync(string id, NuGetVersion version, CancellationToken cancellationToken);
    }
}