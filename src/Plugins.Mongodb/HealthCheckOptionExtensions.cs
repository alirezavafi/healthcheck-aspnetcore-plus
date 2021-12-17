namespace HealthCheck.AspNetCore.Plus.Plugins.Mongodb
{
    public static class HealthCheckOptionExtensions
    {
        public static AppHealthCheckBuilderOptions AddMongodbPlugin(this AppHealthCheckBuilderOptions options)
        {
            options.AddJsonDiscriminator<MongodbHealthCheckItem>("Mongodb");
            return options; 
        }
    }
}