namespace ICanHasDotnetCore
{
    public class KnownReplacements
    {
        private static readonly string[] KnownReplacements = {

        };

        public static Option<PackageResult> Check(string id)
        {
            switch (id)
            {
                case "Microsoft.Bcl":
                    return PackageResult.KnownReplacement(id, "This package was introduced to provide async support to pre-.NET 4.5 platforms", "https://blogs.msdn.microsoft.com/bclteam/2012/10/22/using-asyncawait-without-net-framework-4-5/");
                case "Microsoft.Bcl.Build":
                    return PackageResult.KnownReplacement(id, "This package provided build time support for Microsoft.Bcl", "https://blogs.msdn.microsoft.com/bclteam/2012/10/22/using-asyncawait-without-net-framework-4-5/");
                case "Microsoft.Net.Http":
                case "Microsoft.Bcl.Async":
            }


            return Option<PackageResult>.ToNone;
        }
    }
}