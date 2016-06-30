using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Controllers
{
    [Route("/")]
    public class IndexController : Controller
    {
        public ActionResult Get()
        {
            return File("~/index.html", "text/html");
        }
    }
}