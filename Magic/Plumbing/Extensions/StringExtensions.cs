using System;
using System.Collections.Generic;

namespace ICanHasDotnetCore.Plumbing.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsOrdinalIgnoreCase(this string str, string value) 
            => str.Equals(value, StringComparison.OrdinalIgnoreCase);
        public static bool StartsWithOrdinalIgnoreCase(this string str, string value)
            => str.StartsWith(value, StringComparison.OrdinalIgnoreCase);

        public static string CommaSeperate(this IEnumerable<object> items) => string.Join(", ", items);
    }
}