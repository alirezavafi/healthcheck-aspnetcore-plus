namespace HealthCheck.AspNetCore.Plus.Plugins.MySql
{
    public static class HealthCheckOptionExtensions
    {
        public static AppHealthCheckBuilderOptions AddMySqlPlugin(this AppHealthCheckBuilderOptions options)
        {
            options.AddHealthCheckItemDiscriminator<MySqlHealthCheckItem>("MySql");
            return options; 
        }
    }
}