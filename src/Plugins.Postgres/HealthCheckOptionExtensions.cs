namespace HealthCheck.AspNetCore.Plus.Plugins.Postgres
{
    public static class HealthCheckOptionExtensions
    {
        public static AppHealthCheckBuilderOptions AddPostgresPlugin(this AppHealthCheckBuilderOptions options)
        {
            options.AddJsonDiscriminator<PostgresHealthCheckItem>("Postgres");
            return options; 
        }
    }
}