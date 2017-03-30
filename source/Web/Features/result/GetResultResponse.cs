using System.Collections.Generic;
using ICanHasDotnetCore.NugetPackages;

namespace ICanHasDotnetCore.Web.Features.result
{
    public class GetResultResponse
    {
        public PackageResult[] Result { get; set; }
        public string GraphViz { get; set; }
        public string Cypher { get; set; }
    }

    public class PackageResult
    {
        public string PackageName { get; set; }
        public string Error { get; set; }
        public SupportType SupportType { get; set; }
        public string[] Dependencies { get; set; }
        public string ProjectUrl { get; set; }
        public MoreInformation MoreInformation { get; set; }
    }

}