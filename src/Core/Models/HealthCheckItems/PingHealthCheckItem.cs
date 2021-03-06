using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    [FileDataSourceDiscriminator(Type)]
    public class PingHealthCheckItem : HealthCheckItem
    {
        public PingHealthCheckItem()
        {
            this.Name = Type;
            this.Tags = new[] {"Network", "Infrastructure"};
        }
        public string Host { get; set;}
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        
        public const string Type = "Ping";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddPingHealthCheck(
               o => o.AddHost(Host, Timeout.GetValueOrDefault().Milliseconds), Name, FailureStatus, Tags, Timeout);
        }
    }
}