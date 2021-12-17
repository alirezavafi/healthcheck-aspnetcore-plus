namespace HealthCheck.AspNetCore.Plus.Plugins.SqlServer
{
    public static class HealthCheckOptionExtensions
    {
        public static AppHealthCheckBuilderOptions AddSqlServerPlugin(this AppHealthCheckBuilderOptions options)
        {
            options.AddJsonDiscriminator<SqlServerHealthCheckItem>("SqlServer");
            return options; 
        }
    }
}