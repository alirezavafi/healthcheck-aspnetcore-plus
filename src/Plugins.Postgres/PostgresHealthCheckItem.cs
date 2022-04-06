using System;
using HealthCheck.AspNetCore.Plus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Plugins.Postgres
{
    [FileDataSourceDiscriminator(Type)]
    public class PostgresHealthCheckItem : HealthCheckItem
    {
        public PostgresHealthCheckItem()
        {
            this.Name = Type;
            this.Tags = new[] {"Database"};
        }
        
        public string ConnectionString { get; set;}
        public string HealthQuery { get; set; } = "SELECT 1;";
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        
        public const string Type = "Postgres";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddNpgSql(ConnectionString, HealthQuery, null, Name, FailureStatus, Tags, Timeout);
        }
    }
}