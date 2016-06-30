using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Controllers
{
    [Route("{*url}")]
    public class IndexController : Controller
    {
        public ActionResult Get(string url)
        {
            if(url?.StartsWith("api") ?? false)
                return NotFound();

            return File("~/index.html", "text/html");
        }
    }
}