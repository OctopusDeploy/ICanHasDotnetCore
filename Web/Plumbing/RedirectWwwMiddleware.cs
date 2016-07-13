using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ICanHasDotnetCore.Web.Plumbing
{
    public class RedirectWwwMiddleware
    {
        readonly RequestDelegate _next;

        public RedirectWwwMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Host.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                var withoutWww = Uri.UriSchemeHttps + Uri.SchemeDelimiter + context.Request.Host.Host.Substring(4) + context.Request.Path;
                context.Response.Redirect(withoutWww);
            }
            else
            {
                await _next(context);
            }
        }
    }
}