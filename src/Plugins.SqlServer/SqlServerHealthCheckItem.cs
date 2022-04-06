using System;
using HealthCheck.AspNetCore.Plus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Plugins.SqlServer
{
    [FileDataSourceDiscriminator(Type)]
    public class SqlServerHealthCheckItem : HealthCheckItem
    {
        public SqlServerHealthCheckItem()
        {
            this.Name = Type;
            this.Tags = new[] {"Database"};
        }
        
        public string ConnectionString { get; set;}
        public string HealthQuery { get; set;} = "SELECT 1;";
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        
        public const string Type = "SqlServer";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddSqlServer(ConnectionString, HealthQuery, Name, FailureStatus, Tags, Timeout);
        }
    }
}