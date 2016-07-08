using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ICanHasDotnetCore.Web.Features.Analytics
{
    public class AnalyticsController : Controller
    {
        private readonly IConfigurationRoot _configurationRoot;

        public AnalyticsController(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }
        [HttpGet("api/Analytics")]
        public string Get()
        {
            return _configurationRoot["GoogleAnalytics"];
        }
    }
}