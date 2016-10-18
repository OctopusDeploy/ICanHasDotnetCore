using Autofac;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Web.Features.result.Cache;
using ICanHasDotnetCore.Web.Features.result.GitHub;
using ICanHasDotnetCore.Web.Features.Statistics;

namespace ICanHasDotnetCore.Web.Features
{
    public class FeaturesAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StatisticsRepository>().As<IStatisticsRepository>().SingleInstance();
            builder.RegisterType<GitHubScanner>().AsSelf().SingleInstance();
            builder.RegisterType<RequerySupportTypeForStatisticsPackagesTask>().As<IStartable>().SingleInstance();
            builder.RegisterType<DbNugetResultCache>().As<INugetResultCache>().SingleInstance();
        }
    }
}