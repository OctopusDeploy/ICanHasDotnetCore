using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Controllers
{
    [Route("{*url}")]
    public class IndexController : Controller
    {
        private static readonly string[] NotFoundPaths = new[] { "api", "images", "app" };
        public ActionResult Get(string url)
        {
            if (url != null && NotFoundPaths.Any(url.StartsWith))
                return NotFound();

            return File("~/index.html", "text/html");
        }
    }
}