using System;
using HealthCheck.AspNetCore.Plus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Plugins.Mongodb
{
    public class MongodbHealthCheckItem : HealthCheckItem
    {
        public MongodbHealthCheckItem()
        {
            this.Name = this.Type;
            this.Tags = new[] {"Database"};
        }
        public string ConnectionString { get; set;}
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        
        public sealed override string Type => "Mongodb";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddMongoDb(ConnectionString, Name, FailureStatus, Tags, Timeout);
        }
    }
}