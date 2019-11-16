using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging.Core;

namespace ICanHasDotnetCore.NugetPackages
{
    public interface IPackage
    {
        PackageIdentity Identity { get; }

        Task<NugetPackage> GetNugetPackageAsync(CancellationToken cancellationToken);
    }
}