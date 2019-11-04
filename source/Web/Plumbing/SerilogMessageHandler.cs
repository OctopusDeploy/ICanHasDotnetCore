using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

namespace ICanHasDotnetCore.Web.Plumbing
{
    public class SerilogMessageHandler : DelegatingHandler
    {
        private readonly ILogger _logger;
        private readonly MessageTemplate _messageTemplate;

        public SerilogMessageHandler(ILogger logger, HttpMessageHandler innerHandler = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageTemplate = new MessageTemplateParser().Parse("HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms");
            InnerHandler = innerHandler ?? new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            var statusCode = (int)response.StatusCode;
            var level = HttpLogging.GetLevelForStatusCode(statusCode);
            if (_logger.IsEnabled(level))
            {
                var properties = new List<LogEventProperty>
                {
                    new LogEventProperty("RequestMethod", new ScalarValue(request.Method)),
                    new LogEventProperty("RequestPath", new ScalarValue(request.RequestUri)),
                    new LogEventProperty("StatusCode", new ScalarValue(statusCode)),
                    new LogEventProperty("Elapsed", new ScalarValue(stopwatch.Elapsed.TotalMilliseconds))
                };
                var logEvent = new LogEvent(DateTimeOffset.Now, level, exception: null, _messageTemplate, properties);
                _logger.Write(logEvent);
            }
            return response;
        }
    }
}