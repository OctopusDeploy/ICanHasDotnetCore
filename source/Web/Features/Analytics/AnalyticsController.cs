using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ICanHasDotnetCore.Web.Features.Analytics
{
    public class AnalyticsController : Controller
    {
        private readonly IConfiguration _configuration;

        public AnalyticsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet("api/Analytics")]
        public string Get()
        {
            return _configuration["GoogleAnalytics"];
        }
    }
}