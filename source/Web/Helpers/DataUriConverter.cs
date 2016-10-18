using System;
using System.Linq;
using System.Text;
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