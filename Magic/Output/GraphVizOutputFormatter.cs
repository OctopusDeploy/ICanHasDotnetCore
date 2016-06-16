using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICanHasDotnetCore.Magic.NugetPackages;

namespace ICanHasDotnetCore.Magic.Output
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
                return "#A93226";

            switch (result.SupportType)
            {
                case SupportType.Unknown:
                    return "#E59866";
                case SupportType.Supported:
                    return "#A9DFBF";
                case SupportType.Unsupported:
                    return "#F7DC6F";
                case SupportType.KnownReplacementAvailable:
                    return "#BB8FCE";
                case SupportType.InvestigationTarget:
                    return "lightgray";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}