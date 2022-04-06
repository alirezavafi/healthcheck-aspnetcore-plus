namespace HealthCheck.AspNetCore.Plus.Plugins.MySql
{
    public static class HealthCheckOptionExtensions
    {
        public static AppHealthCheckBuilderOptions AddMySqlFileDataSourceType(this AppHealthCheckBuilderOptions options)
        {
            options.AddFileDataSourceDiscriminator<MySqlHealthCheckItem>();
            return options; 
        }
    }
}