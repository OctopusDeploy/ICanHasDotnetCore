using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ICanHasDotnetCore.Web.Plumbing
{
    public class RedirectWwwMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var request = context.Request;
            if (request.Host.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                var withoutWww = request.Scheme + Uri.SchemeDelimiter + request.Host.Host.Substring(4) + request.Path;
                context.Response.Redirect(withoutWww);
            }
            else
            {
                await next(context);
            }
        }
    }
}