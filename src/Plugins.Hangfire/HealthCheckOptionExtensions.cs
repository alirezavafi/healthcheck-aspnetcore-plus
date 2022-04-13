using HealthCheck.AspNetCore.Plus;

namespace HealthCheck.AspNetCore.Plus.Plugins.Hangfire
{
    public static class HealthCheckOptionExtensions
    {
        public static AppHealthCheckBuilderOptions AddHangfireFileDataSourceType(this AppHealthCheckBuilderOptions options)
        {
            options.AddFileDataSourceDiscriminator<HangfireHealthCheckItem>();
            return options; 
        }
    }
}