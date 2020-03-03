using System.Collections.Generic;
using System.Runtime.Versioning;
using ICanHasDotnetCore.Plumbing;
using NuGet.Versioning;

namespace ICanHasDotnetCore.NugetPackages
{
    public class NugetPackage
    {
        public NugetPackage(string id, IReadOnlyList<string> dependencies, SupportType supportType, Option<NuGetVersion> version, IReadOnlyList<FrameworkName> frameworks)
        {
            Id = id;
            Dependencies = dependencies;
            SupportType = supportType;
            Version = version;
            Frameworks = frameworks;
        }

        public IReadOnlyList<string> Dependencies { get; }
        public string Id { get; }
        public SupportType SupportType { get; }
        public Option<NuGetVersion> Version { get; }
        public string ProjectUrl { get; set; }
        public bool IsPrerelease => Version.IfSome(v => v.IsPrerelease.Some()).ValueOr(false);
        public IReadOnlyList<FrameworkName> Frameworks { get; }
    }
}