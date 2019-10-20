using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace ICanHasDotnetCore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureLogging((context, logging) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .WriteTo.Console(LogEventLevel.Debug)
                        .WriteTo.Seq(context.Configuration["Seq:Url"], apiKey: context.Configuration["Seq:ApiKey"])
                        .Enrich.WithProperty("Application", "ICanHasDotnetCore")
                        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                        .CreateLogger();
                    logging.AddSerilog(Log.Logger);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                })
                .Build();

            host.Run();
        }
    }
}
