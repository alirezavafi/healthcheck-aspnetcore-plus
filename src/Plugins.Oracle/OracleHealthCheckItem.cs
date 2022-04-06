using System;
using HealthCheck.AspNetCore.Plus;
using HealthCheck.AspNetCore.Plus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.Plus.Plugins.MySql
{
    [FileDataSourceDiscriminator(Type)]
    public class OracleHealthCheckItem : HealthCheckItem
    {
        public OracleHealthCheckItem()
        {
            this.Name = Type;
            this.Tags = new[] {"Database"};
        }
        
        public string ConnectionString { get; set;}
        public string HealthQuery { get; set; } = "select * from v$version";
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        
        public const string Type = "Oracle";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddOracle(ConnectionString, HealthQuery, Name, FailureStatus, Tags, Timeout);
        }
    }
}