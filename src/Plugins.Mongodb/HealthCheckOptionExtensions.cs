namespace HealthCheck.AspNetCore.Plus.Plugins.Mongodb
{
    public static class HealthCheckOptionExtensions
    {
        public static AppHealthCheckBuilderOptions AddMongoDbFileDataSourceType(this AppHealthCheckBuilderOptions options)
        {
            options.AddFileDataSourceDiscriminator<MongodbHealthCheckItem>();
            return options; 
        }
    }
}