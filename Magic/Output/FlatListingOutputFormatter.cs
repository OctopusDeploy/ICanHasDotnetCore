using System;
using System.Text;
using ICanHasDotnetCore.Magic.NugetPackages;

namespace ICanHasDotnetCore.Magic.Output
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
            if (result.WasSuccessful)
            {
                switch (result.SupportType)
                {
                    case SupportType.Unknown:
                        sb.AppendLine("[Unknown Support]");
                        break;
                    case SupportType.Supported:
                        sb.AppendLine("[Supported]");
                        break;
                    case SupportType.Unsupported:
                        sb.AppendLine("[Unsupported]");
                        break;
                    case SupportType.KnownReplacementAvailable:
                        sb.AppendLine("[Known Replacement Available]");
                        break;
                    case SupportType.InvestigationTarget:
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            else
            {
                sb.AppendLine(result.Error);
            }
        }
    }
}