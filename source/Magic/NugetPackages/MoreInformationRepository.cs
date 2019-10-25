using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ICanHasDotnetCore.Plumbing;

namespace ICanHasDotnetCore.NugetPackages
{
    public class MoreInformationRepository
    {
        static MoreInformationRepository()
        {
            using (var s = typeof(KnownReplacementsRepository).Assembly.GetManifestResourceStream($"Magic.NugetPackages.Data.MoreInformation.json"))
            using (var sr = new StreamReader(s))
            {
                All = JsonSerializer.Deserialize<MoreInformation[]>(sr.ReadToEnd(), new JsonSerializerOptions {PropertyNameCaseInsensitive = true})
                    .OrderBy(r => r.Id)
                    .ThenBy(r => r.StartsWith) // false first
                    .ToArray();
            }
        }

        public static readonly IReadOnlyList<MoreInformation> All;

        public static Option<MoreInformation> Get(string id) => All.FirstOrNone(k => k.AppliesTo(id));
    }
}