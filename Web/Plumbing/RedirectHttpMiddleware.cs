using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ICanHasDotnetCore.Web.Plumbing
{
    public class RedirectHttpMiddleware
    {
        readonly RequestDelegate _next;

        public RedirectHttpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.IsHttps)
            {
                await _next(context);
            }
            else
            {
                var withHttps = Uri.UriSchemeHttps + Uri.SchemeDelimiter + context.Request.Host + context.Request.Path;
                context.Response.Redirect(withHttps);
            }
        }
    }
}