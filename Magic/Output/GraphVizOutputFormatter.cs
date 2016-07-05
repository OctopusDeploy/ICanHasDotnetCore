using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICanHasDotnetCore.NugetPackages;

namespace ICanHasDotnetCore.Output
{
    public class GraphVizOutputFormatter
    {
        public static string Format(InvestigationResult investigationResult, int levels = Int32.MaxValue)
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
            sb.AppendLine("# This file can be used with GraphVis or http://www.webgraphviz.com/");
            sb.AppendLine($"digraph \"{name}\" {{");
            sb.AppendLine("    graph[layout = fdp];");
            sb.AppendLine("    node[style = filled,shape=box];");
          
            foreach (var result in results)
                foreach (var dep in result.Dependencies)
                    sb.AppendLine($@"    ""{result.PackageName}"" -> ""{dep.PackageName}"";");
            foreach (var result in results)
                sb.AppendLine($@"    ""{result.PackageName}"" [color=""{GetResultColour(result)}""];");

            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GetResultColour(PackageResult result)
        {
            if (!result.WasSuccessful)
                return "#ef9a9a"; // Red

            switch (result.SupportType)
            {
                case SupportType.Unknown:
                    return "#E0E0E0"; // Grey
                case SupportType.Supported:
                    return "#A5D6A7"; // green
                case SupportType.PreRelease:
                    return "#A9DFBF";
                case SupportType.Unsupported:
                    return "#FFCC80"; // orange
                case SupportType.KnownReplacementAvailable:
                    return "#81D4FA"; //blue
                case SupportType.InvestigationTarget:
                    return "#B39DDB"; // purple
                default:
                    return "#FAFAFA"; // B&W
            }
        }
    }
}