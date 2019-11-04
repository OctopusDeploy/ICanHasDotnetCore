using System;
using Autofac;
using ICanHasDotnetCore.Web.Configuration;
using ICanHasDotnetCore.Web.Features;
using ICanHasDotnetCore.Web.Plumbing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Octokit;
using Octokit.Internal;
using Serilog;
using Serilog.Core;

namespace ICanHasDotnetCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to configure the container.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new FeaturesAutofacModule());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddControllers();

            services.AddTransient<RedirectWwwMiddleware>();

            services.AddOptions<AnalyticsSettings>().Bind(Configuration.GetSection("Analytics"));
            services.AddOptions<DatabaseSettings>().Bind(Configuration.GetSection("Database"))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.ConnectionString), "The configuration setting 'Database.ConnectionString' must be set.");
            services.AddOptions<GitHubSettings>().Bind(Configuration.GetSection("GitHub"));
            services.AddSingleton<IAnalyticsSettings>(provider => provider.GetRequiredService<IOptions<AnalyticsSettings>>().Value);
            services.AddSingleton<IDatabaseSettings>(provider => provider.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            services.AddSingleton<IGitHubSettings>(provider => provider.GetRequiredService<IOptions<GitHubSettings>>().Value);

            services.AddSingleton<IGitHubClient>(provider =>
            {
                var settings = provider.GetRequiredService<IGitHubSettings>();
                var credentialStore = new InMemoryCredentialStore(string.IsNullOrEmpty(settings.Token)
                    ? Credentials.Anonymous
                    : new Credentials(settings.Token));
                var productInformation = new ProductHeaderValue("ICanHasDot.net", typeof(Startup).Assembly.GetName().Version.ToString());
                var logger = Log.Logger.ForContext(Constants.SourceContextPropertyName, "Octokit");
                var httpClient = new HttpClientAdapter(() => new SerilogMessageHandler(logger));
                var connection = new Connection(productInformation, GitHubClient.GitHubApiUrl, credentialStore, httpClient, new SimpleJsonSerializer());
                return new GitHubClient(connection);
            });
        }

        private static void ValidateConfiguration(IServiceProvider serviceProvider)
        {
            // Resolve settings in order to force validation and throw if necessary
            try
            {
                serviceProvider.GetRequiredService<IAnalyticsSettings>();
                serviceProvider.GetRequiredService<IDatabaseSettings>();
                serviceProvider.GetRequiredService<IGitHubSettings>();
            }
            catch (Exception exception) when (exception.GetBaseException() is OptionsValidationException validationException)
            {
                // The OptionsValidationException message is terrible: "Exception of type 'Microsoft.Extensions.Options.OptionsValidationException' was thrown."
                throw new Exception($"Validation of {validationException.OptionsType} failed: {string.Join(", ", validationException.Failures)}");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ValidateConfiguration(app.ApplicationServices);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                var context = app.ApplicationServices.GetRequiredService<Database.AppDbContext>();
                context.Database.EnsureCreated();
                context.Dispose();
            }

            app.UseSerilogRequestLogging(options => options.GetLevel = (context, _, __) => HttpLogging.GetLevelForStatusCode(context.Response.StatusCode))
                .UseHttpsRedirection()
                .UseMiddleware<RedirectWwwMiddleware>()
                .UseStaticFiles()
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }

    }
}
