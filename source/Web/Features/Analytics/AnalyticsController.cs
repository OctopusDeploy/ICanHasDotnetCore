using ICanHasDotnetCore.Web.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Features.Analytics
{
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsSettings _analyticsSettings;

        public AnalyticsController(IAnalyticsSettings analyticsSettings)
        {
            _analyticsSettings = analyticsSettings;
        }
        
        [HttpGet("api/Analytics")]
        public string Get()
        {
            return _analyticsSettings.TrackingId;
        }
    }
}