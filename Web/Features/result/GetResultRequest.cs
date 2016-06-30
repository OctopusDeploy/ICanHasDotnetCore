namespace ICanHasDotnetCore.Web.Features.result
{
    public class GetResultRequest
    {
        public PackageFile[] PackageFiles { get; set; }
    }

    public class PackageFile
    {
        public string Name { get; set; }
        public string Contents { get; set; }
    }
}