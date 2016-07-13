using System;

namespace ICanHasDotnetCore.Web.Plumbing.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsOrdinalIgnoreCase(this string str, string value) 
            => str.Equals(value, StringComparison.OrdinalIgnoreCase);
    }
}