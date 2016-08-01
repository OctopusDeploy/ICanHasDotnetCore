using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using NuGet;

namespace ICanHasDotnetCore.NugetPackages
{
    public class PclProfileCompatabilityChecker
    {

        private static readonly NetPortableProfile[] SupportedPclProfiles = new[]
        {
            NetPortableProfile.Parse("net45+netcore45"), // Profile7
            NetPortableProfile.Parse("win81+wp81"), // Profile31
            NetPortableProfile.Parse("win81+wpa81"), // Profile32
            NetPortableProfile.Parse("net451+win81"), // Profile44
            NetPortableProfile.Parse("net45+wp8"), // Profile49
            NetPortableProfile.Parse("net45+win8+wp8"), // Profile78
            NetPortableProfile.Parse("wp81+wpa81"), // Profile84
            NetPortableProfile.Parse("net45+win8+wpa81"), // Profile111
            NetPortableProfile.Parse("net451+win81+wpa81"), // Profile151
            NetPortableProfile.Parse("win81+wp81+wpa81"), // Profile157
            NetPortableProfile.Parse("net45+win8+wpa81+wp8") // Profile259
        };

        public static bool Check(string profileValue)
        {
            profileValue = Regex.Replace(profileValue, @"wpa\+", "wpa81+");
            profileValue = Regex.Replace(profileValue, "wpa$", "wpa81");
            
            var profile = NetPortableProfile.Parse(profileValue);
            return SupportedPclProfiles.Any(p => p.IsCompatibleWith(profile));
        }

    }
}