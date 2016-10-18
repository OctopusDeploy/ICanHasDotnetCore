using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Features.home
{
    [Route("{*url}")]
    public class IndexController : Controller
    {
        private static readonly string[] NotFoundPaths = new[] { "api", "images", "app", "Downloads" };
        public ActionResult Get(string url)
        {
            if (url != null && NotFoundPaths.Any(p => url.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
               return NotFound();

            return File("~/index.html", "text/html");
        }
    }
}