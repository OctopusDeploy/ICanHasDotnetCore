using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NuGet.Common;
using Serilog.Core;
using Serilog.Events;

namespace ICanHasDotnetCore.Investigator
{
    public class NuGetSerilogLogger : LoggerBase
    {
        private readonly Serilog.ILogger _logger;

        public NuGetSerilogLogger() : this(Serilog.Log.Logger.ForContext(Constants.SourceContextPropertyName, "NuGet"))
        {
        }

        public NuGetSerilogLogger(Serilog.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private static readonly Dictionary<LogLevel, LogEventLevel> LogLevelMap = new Dictionary<LogLevel, LogEventLevel>
        {
            [LogLevel.Debug]       = LogEventLevel.Verbose,
            [LogLevel.Verbose]     = LogEventLevel.Debug,
            [LogLevel.Information] = LogEventLevel.Information,
            [LogLevel.Minimal]     = LogEventLevel.Information,
            [LogLevel.Warning]     = LogEventLevel.Warning,
            [LogLevel.Error]       = LogEventLevel.Error,
        };

        public override void Log(ILogMessage message)
        {
            var level = LogLevelMap[message.Level];
            var match = Regex.Match(message.Message, @"(.* )(\d+)ms");
            if (match.Success)
                _logger.Write(level, exception: null, match.Result("$1{time}ms"), int.Parse(match.Groups[2].Value));
            else
                _logger.Write(level, exception: null, message.Message);
        }

        public override Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }
    }
}