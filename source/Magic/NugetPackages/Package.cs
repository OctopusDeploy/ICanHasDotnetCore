using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using ICanHasDotnetCore.Plumbing;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using static NuGet.Frameworks.FrameworkConstants;

namespace ICanHasDotnetCore.NugetPackages
{
    public class Package : IPackage
    {
        // All PCL profiles that support .NET Standard, see https://docs.microsoft.com/en-us/dotnet/standard/net-standard#pcl-compatibility and https://portablelibraryprofiles.stephencleary.com
        private static readonly string[] SupportedPclProfiles = DefaultPortableFrameworkMappings.Instance.CompatibilityMappings
            .Select(e => FrameworkNameHelpers.GetPortableProfileNumberString(e.Key)).ToArray();

        private static readonly string[] SupportedTargetFrameworksInOrderOfPriority =
        {
            FrameworkIdentifiers.NetStandard,
            FrameworkIdentifiers.NetCoreApp,
            FrameworkIdentifiers.DnxCore,
            FrameworkIdentifiers.AspNetCore,
            FrameworkIdentifiers.NetPlatform,
        };

        private readonly string _projectUrl;
        private readonly IReadOnlyList<PackageDependencyGroup> _dependencyGroups;
        private readonly IReadOnlyList<NuGetFramework> _frameworks;
        private readonly Func<PackageIdentity, CancellationToken, Task<PackageReaderBase>> _getPackageReaderAsync;

        public Package(IPackageSearchMetadata metadata, Func<PackageIdentity, CancellationToken, Task<PackageReaderBase>> getPackageReaderAsync)
        {
            Identity = metadata.Identity;
            _projectUrl = metadata.ProjectUrl?.AbsoluteUri;
            _dependencyGroups = metadata.DependencySets.ToList();
            _frameworks = _dependencyGroups.Select(f => NuGetFramework.ParseFrameworkName(f.TargetFramework.DotNetFrameworkName, DefaultFrameworkNameProvider.Instance)).ToList();
            _getPackageReaderAsync = getPackageReaderAsync;
        }

        public PackageIdentity Identity { get; }

        private static Option<NuGetFramework> GetDotNetCoreFramework(IReadOnlyList<NuGetFramework> frameworks)
        {
            foreach (var supportedTargetFramework in SupportedTargetFrameworksInOrderOfPriority)
            {
                var match = frameworks.FirstOrNone(f => f.Framework.Equals(supportedTargetFramework, StringComparison.OrdinalIgnoreCase));
                if (match.Some)
                    return match.Value;
            }
            var pclMatch = frameworks.FirstOrNone(f => f.IsPCL && SupportedPclProfiles.Contains(f.Profile));
            return pclMatch.Some ? pclMatch.Value : Option<NuGetFramework>.ToNone;
        }

        private IEnumerable<PackageDependency> GetDependencies(FrameworkName framework)
        {
            var matchingGroup = _dependencyGroups
                .OrderByDescending(e => e.TargetFramework.Version)
                .FirstOrDefault(e => e.TargetFramework.Framework == framework.Identifier);
            var dependencyGroup = matchingGroup ?? _dependencyGroups.FirstOrDefault();
            return dependencyGroup?.Packages ?? Enumerable.Empty<PackageDependency>();
        }

        private async Task<SupportType> GetSupportTypeAsync(Option<NuGetFramework> dotNetCoreFramework, CancellationToken cancellationToken)
        {
            if (dotNetCoreFramework.Some)
            {
                return Identity.Version.IsPrerelease ? SupportType.PreRelease : SupportType.Supported;
            }

            var packageReader = await _getPackageReaderAsync(Identity, cancellationToken);

            var supportedFrameworks = await packageReader.GetSupportedFrameworksAsync(cancellationToken);
            if (GetDotNetCoreFramework(supportedFrameworks.ToList()).Some)
            {
                return Identity.Version.IsPrerelease ? SupportType.PreRelease : SupportType.Supported;
            }

            var libItems = await packageReader.GetLibItemsAsync(cancellationToken);
            var isDotNetLibrary = libItems.Any();
            return isDotNetLibrary ? SupportType.Unsupported : SupportType.NoDotNetLibraries;
        }

        public async Task<NugetPackage> GetNugetPackageAsync(CancellationToken cancellationToken)
        {
            var dotNetCoreFramework = GetDotNetCoreFramework(_frameworks);
            var frameworkName = dotNetCoreFramework.Some
                ? new FrameworkName(dotNetCoreFramework.Value.DotNetFrameworkName)
                : new FrameworkName(FrameworkIdentifiers.Net, EmptyVersion);
            var dependencies = GetDependencies(frameworkName);
            var filteredDependencies = dependencies
                .Select(d => d.Id)
                .Where(d => !(dotNetCoreFramework.Some && d.StartsWith("System.")))
                .Where(d => !d.StartsWith("Microsoft.NETCore."))
                .Where(d => d != "NETStandard.Library")
                .ToList();
            var supportType = await GetSupportTypeAsync(dotNetCoreFramework, cancellationToken);
            var frameworks = _frameworks.Select(f => new FrameworkName(f.DotNetFrameworkName)).ToList();
            return new NugetPackage(Identity.Id, filteredDependencies, supportType, Identity.Version, frameworks) { ProjectUrl = _projectUrl };
        }
    }
}