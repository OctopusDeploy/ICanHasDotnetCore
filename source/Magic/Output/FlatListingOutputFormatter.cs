using System;
using System.Text;
using ICanHasDotnetCore.Investigator;
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
                    sb.Append("[Not Found]");
                    break;
                case SupportType.Supported:
                    sb.Append("[Supported]");
                    break;
                case SupportType.PreRelease:
                    sb.Append("[Supported (Pre-Release)]");
                    break;
                case SupportType.Unsupported:
                    sb.Append("[Unsupported]");
                    break;
                case SupportType.KnownReplacementAvailable:
                    sb.Append("[Known Replacement Available]");
                    break;
                case SupportType.InvestigationTarget:
                    sb.Append("[Your Project]");
                    break;
                case SupportType.Error:
                    sb.Append(result.Error);
                    break;
                case SupportType.NoDotNetLibraries:
                    sb.Append("[Not a .NET Library]");
                    break;
                default:
                    throw new ArgumentException();
            }

            if (result.MoreInformation.Some)
            {
                var info = result.MoreInformation.Value;

                if (info.Message != null)
                    sb.Append(" ").Append(info.Message);

                if (info.Url != null)
                {
                    sb.Append(" - ");
                    if (info.LinkText != null)
                        sb.Append(info.LinkText).Append(": ");
                    sb.Append(info.Url);
                }
            }

            sb.AppendLine();
        }
    }
}