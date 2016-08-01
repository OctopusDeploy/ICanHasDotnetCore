using System.Collections.Generic;
using System.Runtime.Versioning;
using ICanHasDotnetCore.Plumbing;
using NuGet;

namespace ICanHasDotnetCore.NugetPackages
{
    public class NugetPackage
    {
        public NugetPackage(string id, IReadOnlyList<string> dependencies, SupportType supportType, Option<SemanticVersion> version, IReadOnlyList<FrameworkName> frameworks)
        {
            Id = id;
            Dependencies = dependencies;
            SupportType = supportType;
            Version = version;
            Frameworks = frameworks;
        }

        public IReadOnlyList<string> Dependencies { get;  }
        public string Id { get;  }
        public SupportType SupportType { get;  }
        public Option<SemanticVersion> Version { get;  }
        public string ProjectUrl { get; set; }
        public bool IsPrerelease => Version.IfSome(v => string.IsNullOrEmpty(v.SpecialVersion).Some()).ValueOr(false);
        public IReadOnlyList<FrameworkName> Frameworks { get;  }
    }
}