using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.NugetPackages;

namespace ICanHasDotnetCore.Output
{
    public class CypherOutputFormatter
    {
        public static string Format(InvestigationResult investigationResult, int levels = int.MaxValue)
        {
            var allResults = investigationResult.GetAllDistinctRecursive(levels);
            return Format(allResults, "All");
        }

        public static string Format(PackageResult result)
        {
            var allResults = result.GetDependenciesResursive().Distinct().ToArray();
            return Format(allResults, result.PackageName);
        }

        private static string Format(IReadOnlyList<PackageResult> results, string name)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// This file can be used with any Cypher based DB or on https://neo4j.com/sandbox-v2/ ");
            var identifiersByPackageName = new Dictionary<string, string>();
            for (var i = 0; i < results.Count; i++)
                identifiersByPackageName.Add(results[i].PackageName, $"p{i}");

            var creates = new List<string>();
            foreach (var result in results)
            {
                var identifier = identifiersByPackageName[result.PackageName];
                creates.Add($"({identifier}:Package:{SupportTypeToLabel(result.SupportType)} {{Name:'{EscapeText(result.PackageName)}', ProjectUrl:'{EscapeText(result.ProjectUrl)}' }})");
            }

            foreach (var result in results)
            {
                var fromId = identifiersByPackageName[result.PackageName];
                foreach (var dep in result.Dependencies)
                {
                    var toId = identifiersByPackageName[dep.PackageName];
                    creates.Add($"({fromId})-[:DEPENDS_ON]->({toId})");
                }
            }

            foreach (var result in results.Where(r => r.SupportType == SupportType.KnownReplacementAvailable && r.MoreInformation.Some))
            {
                var id = identifiersByPackageName[result.PackageName];
                var info = result.MoreInformation?.Value;
                if(info != null)
                    creates.Add($"({id})<-[:HAS_KNOWN_REPLACEMENT]-(:Replacement {{Id:'{info.Id}', LinkText:'{EscapeText(info.LinkText)}', Message:'{EscapeText(info.Message)}', Url:'{EscapeText(info.LinkText)}', StartsWith: {info.StartsWith}}})");
            }

            sb.AppendLine("CREATE");
            sb.AppendLine($"\t{string.Join($",{Environment.NewLine}\t", creates)}");
            return sb.ToString();
        }

        private static string SupportTypeToLabel(SupportType supportType)
        {
            switch (supportType)
            {
                case SupportType.NotFound:
                case SupportType.Supported:
                case SupportType.PreRelease:
                case SupportType.Error:
                case SupportType.NoDotNetLibraries:
                case SupportType.InvestigationTarget:
                    return supportType.ToString();
                case SupportType.Unsupported:
                case SupportType.KnownReplacementAvailable:
                    return "Unsupported";
                default:
                    throw new ArgumentOutOfRangeException(nameof(supportType), supportType, null);
            }
        }

        private static string EscapeText(string toEscape)
        {
            return string.IsNullOrWhiteSpace(toEscape) 
                ? toEscape 
                : toEscape.Replace("\\", "\\\\");
        }
    }
}