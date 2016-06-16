using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICanHasDotnetCore.Magic.Output
{
    public class TreeOutputFormatter
    {
        public static string Format(InvestigationResult investigationResult)
        {
            var sb = new StringBuilder();
            Format(sb, investigationResult.PackageConfigResults);
            return sb.ToString();
        }

        public static void Format(StringBuilder sb, IEnumerable<PackageResult> packages, string indent = "")
        {
            foreach (var package in packages)
            {
                sb.Append(indent);
                FlatListingOutputFormatter.Format(sb, package);

                if (package.WasSuccessful)
                    Format(sb, package.Dependencies, indent + "    ");
            }
        }
    }
}