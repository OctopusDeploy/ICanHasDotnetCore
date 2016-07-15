using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.Plumbing.Extensions;
using Newtonsoft.Json;

namespace ICanHasDotnetCore.NugetPackages
{
    public class KnownReplacementsRepository
    {
        static KnownReplacementsRepository()
        {
            using (var s = typeof(KnownReplacementsRepository).Assembly.GetManifestResourceStream($"Magic.NugetPackages.Data.KnownReplacements.json"))
            using (var sr = new StreamReader(s))
            {
                All = JsonConvert.DeserializeObject<MoreInformation[]>(sr.ReadToEnd())
                    .OrderBy(r => r.Id)
                    .ThenBy(r => r.StartsWith) // false first
                    .ToArray();
            }
        }

        public static readonly IReadOnlyList<MoreInformation> All;

        public static Option<MoreInformation> Get(string id) => All.FirstOrNone(k => k.AppliesTo(id));
    }
}