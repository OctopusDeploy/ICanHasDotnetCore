using System.Collections.Generic;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.Plumbing.Extensions;

namespace ICanHasDotnetCore.NugetPackages
{
    public class KnownReplacement
    {
        private const string AspNetUrl = "https://docs.asp.net/en/latest/migration/rc1-to-rtm.html#namespace-and-package-id-changes";
        private const string HttpClientUrl = "https://blogs.msdn.microsoft.com/bclteam/2013/02/18/portable-httpclient-for-net-framework-and-windows-phone/";
        private const string BclUrl = "https://blogs.msdn.microsoft.com/bclteam/2012/10/22/using-asyncawait-without-net-framework-4-5/";
        private const string OwinUrl = "https://docs.asp.net/en/latest/fundamentals/owin.html";

        public static readonly IReadOnlyList<KnownReplacement> All = new[]
        {
            // BCL
            new KnownReplacement(
                "Microsoft.Bcl",
                "This package was introduced to provide async support to pre-.NET 4.5 platforms",
                BclUrl
            ),
            new KnownReplacement(
                "Microsoft.Bcl.Async",
                "This package was introduced to provide async support to pre-.NET 4.5 platforms",
                BclUrl
            ),
            new KnownReplacement(
                "Microsoft.Bcl.Build", "This package provided build time support for Microsoft.Bcl",
                BclUrl
            ),
            new KnownReplacement(
                "Microsoft.Net.Http", "This package backported HttpClient to pre-.NET 4.5 platforms",
                HttpClientUrl
            ),
            new KnownReplacement(
                "Microsoft.Net.Http", "This package backported HttpClient to pre-.NET 4.5 platforms",
                HttpClientUrl
            ),

            // ASP.NET
            new KnownReplacement(
                "EntityFramework.MicrosoftSqlServer",
                "Replaced by Microsoft.EntityFrameworkCore.SqlServer",
                AspNetUrl),
            new KnownReplacement(
                "Microsoft.AspNet.Diagnostics.Entity",
                "Replaced by Microsoft.AspNetCore.Dianostics.EntityFrameworkCore",
                AspNetUrl),
            new KnownReplacement(
                "Microsoft.AspNet.Identity.EntityFramework",
                "Replaced by Microsoft.AspNetCore.Identity.EntityFrameworkCore",
                AspNetUrl),
            new KnownReplacement(
                "Microsoft.AspNet.Tooling.Razor",
                "Replaced by Microsoft.AspNetCore.Razor.Tools",
                AspNetUrl),
            new KnownReplacement(
                "Microsoft.Web.Infrastructure",
                "This package was used to register modules in the ASP.NET pipeline, the ASP.NET Core architecure changes make this obsolete"),
            new KnownReplacement(
                "Owin",
                "Owin has been replaced by Microsoft.AspNetCore.Owin",
                OwinUrl
            ),
            new KnownReplacement(
                "Microsoft.Owin",
                "Owin has been replaced by Microsoft.AspNetCore.Owin",
                OwinUrl
            ),
             new KnownReplacement(
                "Microsoft.Owin.",
                "Owin has been replaced by Microsoft.AspNetCore.Owin",
                OwinUrl,
                startsWith: true
            ),
            new KnownReplacement(
                "Microsoft.AspNet.",
                "Microsoft.AspNet.* packages have been replaced with Microsoft.AspNetCore.*",
                AspNetUrl,
                startsWith: true
            ),
            new KnownReplacement(
                "EntityFramework.",
                "EntityFramework.* packages have been replaced with Microsoft.EntityFrameworkCore.*",
                AspNetUrl,
                startsWith: true
            ),
            new KnownReplacement(
                "Microsoft.Data.Entity.",
                "Microsoft.Data.Entity.* packages have been replaced with Microsoft.EntityFrameworkCore.*",
                AspNetUrl,
                startsWith: true
            )
        };

        public string Id { get; }
        public string Message { get; }
        public string Url { get; }
        public bool StartsWith { get; set; }

        private KnownReplacement(string id, string message, string url = null, bool startsWith = false)
        {
            Id = id;
            Message = message;
            Url = url;
            StartsWith = startsWith;
        }

        public bool AppliesTo(string id) => StartsWith ? id.StartsWithOrdinalIgnoreCase(Id) : id.EqualsOrdinalIgnoreCase(Id);

        public static Option<KnownReplacement> Check(string id) => All.FirstOrNone(k => k.AppliesTo(id));
    }
}