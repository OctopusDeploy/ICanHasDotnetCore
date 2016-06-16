using System.Web;

namespace ICanHasDotnetCore.Web.Models
{
    public class ComputeRequest
    {
        public string[] Name { get; set; }
        public HttpPostedFileBase[] File { get; set; }
    }
}