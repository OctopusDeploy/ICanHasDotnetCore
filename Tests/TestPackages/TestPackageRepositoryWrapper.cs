using System.IO;
using System.Threading.Tasks;
using NuGet;

namespace ICanHasDotnetCore.Tests.TestPackages
{
    public class TestPackageRepositoryWrapper : IPackageRepositoryWrapper
    {
        public Task<IPackage> GetLatestPackage(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Result<byte[]>> DownloadPackage(IPackageName package)
        {
            var type = typeof(TestPackageRepositoryWrapper);
            var resourceName = $"{type.Namespace}.{package.Id}.{package.Version}.nupkg";
            using (var s = type.Assembly.GetManifestResourceStream(resourceName))
            {
                if(s == null)
                    return Result<byte[]>.Failed($"Package {package.Id} {package.Version} not found");

                using (var ms = new MemoryStream())
                {
                    await s.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}