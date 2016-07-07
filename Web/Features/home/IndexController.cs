using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Controllers
{
    [Route("{*url}")]
    public class IndexController : Controller
    {
        public ActionResult Get(string url)
        {
            if(url == null  || url.StartsWith("api") || url.StartsWith("images") || url.StartsWith("app"))
                return NotFound();

            return File("~/index.html", "text/html");
        }
    }
}