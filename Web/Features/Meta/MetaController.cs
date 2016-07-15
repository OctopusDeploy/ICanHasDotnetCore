using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Features.Meta
{
    public class MetaController : Controller
    {

        private string BaseUrl => Request.Scheme + Uri.SchemeDelimiter + Request.Host;

        [HttpGet("robots.txt")]
        public ContentResult RobotsTxt()
        {
            return Content($@"User-agent:*
Sitemap: {BaseUrl}/sitemap.xml", "text/plain", Encoding.UTF8);
        }

        [HttpGet("sitemap.xml")]
        public ContentResult SiteMap()
        {
            var sitemap = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">
    {GetSiteMapPortion("", "0.9")}
    {GetSiteMapPortion("Result/Demo", "0.6")}
    {GetSiteMapPortion("Console")}
    {GetSiteMapPortion("Faq", "0.3")}
    {GetSiteMapPortion("Replacements", "0.3")}
    {GetSiteMapPortion("Stats", changeFreq: "hourly")}
</urlset>";

            return Content(sitemap, "application/xml", Encoding.UTF8);
        }

        private string GetSiteMapPortion(string path, string priority = "0.5", string changeFreq = "daily")
        {
            return $@"<url>
      <loc>{BaseUrl}/{path}</loc>
      <changefreq>{changeFreq}</changefreq>
      <priority>{priority}</priority>
   </url>";
        }
    }
}