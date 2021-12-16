using HealthCheck.AspNetCore.Plus;

namespace HealthCheck.Plus.Plugins.MySql
{
    public static class HealthCheckOptionExtensions
    {
        public static AppHealthCheckBuilderOptions AddOraclePlugin(this AppHealthCheckBuilderOptions options)
        {
            options.AddHealthCheckItemDiscriminator<OracleHealthCheckItem>("Oracle");
            return options; 
        }
    }
}