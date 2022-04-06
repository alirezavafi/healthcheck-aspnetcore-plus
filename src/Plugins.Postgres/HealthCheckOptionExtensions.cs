namespace HealthCheck.AspNetCore.Plus.Plugins.Postgres
{
    public static class HealthCheckOptionExtensions
    {
        public static AppHealthCheckBuilderOptions AddPostgresFileDataSourceType(this AppHealthCheckBuilderOptions options)
        {
            options.AddFileDataSourceDiscriminator<PostgresHealthCheckItem>();
            return options; 
        }
    }
}