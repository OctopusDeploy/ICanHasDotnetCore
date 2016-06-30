using System;
using System.Linq;
using System.Text;
namespace ICanHasDotnetCore.Web.Helpers
{
    public static class DataUriConverter
    {
        public static string ConvertFrom(string str)
        {
            var data = Convert.FromBase64String(str.Split(',')[1]);
            return Encoding.UTF8.GetString(data);
        }
    }
}