using System.Collections.Generic;
using ICanHasDotnetCore.Plumbing;
using NuGet;

namespace ICanHasDotnetCore.NugetPackages
{
    public class NugetPackage
    {
        protected NugetPackage()
        {
            
        }
        public NugetPackage(string id, IReadOnlyList<string> dependencies, SupportType supportType, Option<SemanticVersion> version)
        {
            Id = id;
            Dependencies = dependencies;
            SupportType = supportType;
            Version = version;
        }

        public IReadOnlyList<string> Dependencies { get; set; }
        public string Id { get; set; }
        public SupportType SupportType { get; set; }
        public Option<SemanticVersion> Version { get; set; }
        public string ProjectUrl { get; set; }
        public bool IsPrerelease => Version.IfSome(v => string.IsNullOrEmpty(v.SpecialVersion).Some()).ValueOr(false);
    }
}