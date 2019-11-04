using Serilog.Events;

namespace ICanHasDotnetCore.Web.Plumbing
{
    public static class HttpLogging
    {
        public static LogEventLevel GetLevelForStatusCode(int statusCode)
        {
            if (statusCode >= 500)
                return LogEventLevel.Error;
            if (statusCode >= 400)
                return LogEventLevel.Warning;
            return LogEventLevel.Information;
        }
    }
}