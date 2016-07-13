using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ICanHasDotnetCore.Web.Plumbing.Extensions;

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
            if (context.Request.IsHttps || context.Request.Host.Host.EqualsOrdinalIgnoreCase("localhost"))
            {
                await _next(context);
            }
            else
            {
                var withHttps = Uri.UriSchemeHttps + Uri.SchemeDelimiter + context.Request.Host.Host + context.Request.Path;
                context.Response.Redirect(withHttps);
            }
        }
    }
}