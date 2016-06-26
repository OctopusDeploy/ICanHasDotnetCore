using System;
using System.Collections.Generic;
using System.Linq;
using ICanHasDotnetCore.NugetPackages;

namespace ICanHasDotnetCore
{
    public class PackageResult
    {
        private PackageResult()
        {
        }

        public string PackageName { get; private set; }
        public string Error { get; private set; }
        public IReadOnlyList<PackageResult> Dependencies { get; private set; }
        public bool WasSuccessful { get; private set; }
        public SupportType SupportType { get; private set; }

        public static PackageResult Failed(string packageName, string error)
        {
            return new PackageResult()
            {
                PackageName = packageName,
                Error = error,
                Dependencies = new PackageResult[0]
            };
        }

        public static PackageResult Success(string packageName, IReadOnlyList<PackageResult> dependencies, SupportType supportType)
        {
            return new PackageResult()
            {
                PackageName = packageName,
                Dependencies = dependencies,
                WasSuccessful = true,
                SupportType = supportType
            };
        }

        public IEnumerable<PackageResult> GetDependenciesResursive(int maxLevels = Int32.MaxValue)
        {
            if (maxLevels == 0)
                return Dependencies;
            return Dependencies.Concat(Dependencies.SelectMany(d => d.GetDependenciesResursive(maxLevels-1)));
        }
    }
}