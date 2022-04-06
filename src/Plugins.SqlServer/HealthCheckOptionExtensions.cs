namespace HealthCheck.AspNetCore.Plus.Plugins.SqlServer
{
    public static class HealthCheckOptionExtensions
    {
        public static AppHealthCheckBuilderOptions AddSqlServerFileDataSourceType(this AppHealthCheckBuilderOptions options)
        {
            options.AddFileDataSourceDiscriminator<SqlServerHealthCheckItem>();
            return options; 
        }
    }
}