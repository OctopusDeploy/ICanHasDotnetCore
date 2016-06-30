using System.Collections.Generic;

namespace ICanHasDotnetCore.NugetPackages
{
    public class NugetPackage
    {
        protected NugetPackage()
        {
            
        }
        public NugetPackage(string id, IReadOnlyList<string> dependencies, SupportType supportType)
        {
            Id = id;
            Dependencies = dependencies;
            SupportType = supportType;
        }

        public IReadOnlyList<string> Dependencies { get; set; }
        public string Id { get; set; }
        public SupportType SupportType { get; set; }
    }
}