using System;
using System.Text;
using ICanHasDotnetCore.NugetPackages;

namespace ICanHasDotnetCore.Output
{
    public class FlatListingOutputFormatter
    {
        public static string Format(InvestigationResult investigationResult)
        {
            var sb = new StringBuilder();
            foreach (var result in investigationResult.GetAllDistinctRecursive())
            {
                Format(sb, result);
            }
            return sb.ToString();
        }

        internal static void Format(StringBuilder sb, PackageResult result)
        {
            sb.Append(result.PackageName).Append("   ");
            switch (result.SupportType)
            {
                case SupportType.NotFound:
                    sb.AppendLine("[Not Found]");
                    break;
                case SupportType.Supported:
                    sb.AppendLine("[Supported]");
                    break;
                case SupportType.PreRelease:
                    sb.AppendLine("[Supported (Pre-Release)]");
                    break;
                case SupportType.Unsupported:
                    sb.AppendLine("[Unsupported]");
                    break;
                case SupportType.KnownReplacementAvailable:
                    sb.AppendLine("[Known Replacement Available]");
                    break;
                case SupportType.InvestigationTarget:
                    sb.AppendLine("[Your Project]");
                    break;
                case SupportType.Error:
                    sb.AppendLine(result.Error);
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}