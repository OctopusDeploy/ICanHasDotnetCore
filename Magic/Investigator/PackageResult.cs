using System;
using System.Collections.Generic;
using System.Linq;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Plumbing;

namespace ICanHasDotnetCore.Investigator
{
    public class PackageResult
    {
        private PackageResult()
        {
        }

        public string PackageName { get; private set; }
        public string ProjectUrl { get; private set; }
        public string Error { get; private set; }
        public IReadOnlyList<PackageResult> Dependencies { get; private set; }
        public bool WasSuccessful => SupportType != SupportType.Error;
        public SupportType SupportType { get; private set; }

        public Option<MoreInformation> MoreInformation { get; set; }

        public static PackageResult Failed(string packageName, string error)
        {
            return new PackageResult()
            {
                PackageName = packageName,
                Error = error,
                Dependencies = new PackageResult[0],
                SupportType = SupportType.Error,
                MoreInformation = Option<MoreInformation>.ToNone
            };
        }

        public static PackageResult InvestigationTarget(string name, IReadOnlyList<PackageResult> dependencies)
        {
            return new PackageResult()
            {
                PackageName = name,
                Dependencies = dependencies,
                SupportType = SupportType.InvestigationTarget,
            };
        }

        public static PackageResult KnownReplacement(string name, MoreInformation moreInformation)
        {
            return new PackageResult()
            {
                PackageName = name,
                Dependencies = new PackageResult[0],
                SupportType = SupportType.KnownReplacementAvailable,
                MoreInformation = moreInformation
            };
        }



        public static PackageResult Success(NugetPackage package, IReadOnlyList<PackageResult> dependencies, Option<MoreInformation> moreInformation)
        {
            return new PackageResult()
            {
                PackageName = package.Id,
                Dependencies = dependencies,
                SupportType = package.SupportType,
                ProjectUrl = package.ProjectUrl,
                MoreInformation = moreInformation
            };
        }

        public IEnumerable<PackageResult> GetDependenciesResursive(int maxLevels = Int32.MaxValue)
        {
            if (maxLevels == 0)
                return Dependencies;
            return Dependencies.Concat(Dependencies.SelectMany(d => d.GetDependenciesResursive(maxLevels - 1)));
        }
    }
}