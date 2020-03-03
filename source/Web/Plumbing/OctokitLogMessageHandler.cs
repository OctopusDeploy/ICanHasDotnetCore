using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cashew.Headers;
using Octokit;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace ICanHasDotnetCore.Web.Plumbing
{
    public class OctokitLogMessageHandler : DelegatingHandler
    {
        private static readonly ILogger Logger = Log.Logger.ForContext(Constants.SourceContextPropertyName, "Octokit");
        private static readonly MessageTemplate MessageTemplate = new MessageTemplateParser().Parse("HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms (cache {CacheStatus}, rate limit {RateLimit})");

        public OctokitLogMessageHandler(HttpMessageHandler innerHandler)
        {
            InnerHandler = innerHandler ?? throw new ArgumentNullException(nameof(innerHandler));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            var statusCode = (int)response.StatusCode;
            var level = HttpLogging.GetLevelForStatusCode(statusCode);
            if (Logger.IsEnabled(level))
            {
                var cacheStatus = response.Headers.GetCashewStatusHeader();
                var rateLimit = new RateLimit(response.Headers.ToDictionary(e => e.Key, e => string.Join(";", e.Value)));
                var properties = new[]
                {
                    new LogEventProperty("RequestMethod", new ScalarValue(request.Method)),
                    new LogEventProperty("RequestPath", new ScalarValue(request.RequestUri)),
                    new LogEventProperty("StatusCode", new ScalarValue(statusCode)),
                    new LogEventProperty("Elapsed", new ScalarValue(stopwatch.Elapsed.TotalMilliseconds)),
                    new LogEventProperty("CacheStatus", new ScalarValue(cacheStatus)),
                    new LogEventProperty("RateLimit", new StructureValue(new[]
                    {
                        new LogEventProperty(nameof(rateLimit.Limit), new ScalarValue(rateLimit.Limit)),
                        new LogEventProperty(nameof(rateLimit.Remaining), new ScalarValue(rateLimit.Remaining)),
                        new LogEventProperty(nameof(rateLimit.Reset), new ScalarValue(rateLimit.Reset)),
                    })),
                };
                var logEvent = new LogEvent(DateTimeOffset.Now, level, exception: null, MessageTemplate, properties);
                Logger.Write(logEvent);
            }
            return response;
        }
    }
}