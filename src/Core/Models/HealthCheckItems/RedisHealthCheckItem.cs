using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    [FileDataSourceDiscriminator(Type)]
    public class RedisHealthCheckItem : HealthCheckItem
    {
        public RedisHealthCheckItem()
        {
            this.Name = Type;
            this.Tags = new[] {"Database", "Cache"};
        }
        public string ConnectionString { get; set;}
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        
        public const string Type = "Redis";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddRedis(ConnectionString, Name, FailureStatus, Tags, Timeout);
        }
    }
}