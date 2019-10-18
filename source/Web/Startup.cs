using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ICanHasDotnetCore.Web.Configuration;
using ICanHasDotnetCore.Web.Plumbing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ICanHasDotnetCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddTransient<RedirectWwwMiddleware>();

            services.AddOptions<AnalyticsSettings>().Bind(Configuration.GetSection("Analytics"));
            services.AddOptions<DatabaseSettings>().Bind(Configuration.GetSection("Database"))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.ConnectionString), "The configuration setting 'Database.ConnectionString' must be set.");
            services.AddOptions<GitHubSettings>().Bind(Configuration.GetSection("GitHub"));
            services.AddSingleton<IAnalyticsSettings>(provider => provider.GetRequiredService<IOptions<AnalyticsSettings>>().Value);
            services.AddSingleton<IDatabaseSettings>(provider => provider.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            services.AddSingleton<IGitHubSettings>(provider => provider.GetRequiredService<IOptions<GitHubSettings>>().Value);
            
            // Create the container builder.
            var builder = new ContainerBuilder();

            // Register dependencies, populate the services from
            // the collection, and build the container.
            builder.RegisterAssemblyModules(typeof(Startup).Assembly);
            builder.Populate(services);
            var container = builder.Build();

            // Resolve settings in order to force validation and throw if necessary
            try
            {
                container.Resolve<IAnalyticsSettings>();
                container.Resolve<IDatabaseSettings>();
                container.Resolve<IGitHubSettings>();
            }
            catch (Exception exception) when (exception.GetBaseException() is OptionsValidationException validationException)
            {
                // The OptionsValidationException message is terrible: "Exception of type 'Microsoft.Extensions.Options.OptionsValidationException' was thrown."
                throw new Exception($"Validation of {validationException.OptionsType} failed: {string.Join(", ", validationException.Failures)}");
            }
            
            // Return the IServiceProvider resolved from the container.
            return container.Resolve<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection()
                .UseMiddleware<RedirectWwwMiddleware>()
                .UseStaticFiles()
                .UseMvc();
        }

    }
}
