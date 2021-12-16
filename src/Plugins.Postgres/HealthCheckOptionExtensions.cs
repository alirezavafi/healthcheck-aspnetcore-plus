namespace HealthCheck.AspNetCore.Plus.Plugins.Postgres
{
    public static class HealthCheckOptionExtensions
    {
        public static AppHealthCheckBuilderOptions AddPostgresPlugin(this AppHealthCheckBuilderOptions options)
        {
            options.AddHealthCheckItemDiscriminator<PostgresHealthCheckItem>("Postgres");
            return options; 
        }
    }
}