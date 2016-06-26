using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICanHasDotnetCore.Web.Models;

namespace ICanHasDotnetCore.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ComputeRequest request)
        {
            var inputs = request.Name
                .Zip(request.File, (n, f) => new PackagesFileData(n, Read(f)))
                .ToArray();
            return View("Result", PackageCompatabilityInvestigator.Create().Go(inputs));
        }

        private string Read(HttpPostedFileBase postedFile)
        {
            if (postedFile == null)
                return null;
            return new StreamReader(postedFile.InputStream)
                .ReadToEnd();
        }
    }
}