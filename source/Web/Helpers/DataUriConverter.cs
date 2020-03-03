using System;

namespace ICanHasDotnetCore.Web.Helpers
{
    public static class DataUriConverter
    {
        public static byte[] ConvertFrom(string str)
        {
            return Convert.FromBase64String(str.Split(',')[1]);
        }
    }
}