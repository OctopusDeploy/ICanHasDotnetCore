using Autofac;
using ICanHasDotnetCore.Web.Features.result.GitHub;
using ICanHasDotnetCore.Web.Features.Statistics;

namespace ICanHasDotnetCore.Web.Features
{
    public class FeaturesAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StatisticsRepository>().AsSelf().SingleInstance();
            builder.RegisterType<GitHubScanner>().AsSelf().SingleInstance();
            builder.RegisterType<RequeryStatisticsPackageSupport>().As<IStartable>().SingleInstance();
        }
    }
}