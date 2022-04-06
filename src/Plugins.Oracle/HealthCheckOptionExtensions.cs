using HealthCheck.AspNetCore.Plus;

namespace HealthCheck.Plus.Plugins.MySql
{
    public static class HealthCheckOptionExtensions
    {
        public static AppHealthCheckBuilderOptions AddOracleFileDataSourceType(this AppHealthCheckBuilderOptions options)
        {
            options.AddFileDataSourceDiscriminator<OracleHealthCheckItem>();
            return options; 
        }
    }
}