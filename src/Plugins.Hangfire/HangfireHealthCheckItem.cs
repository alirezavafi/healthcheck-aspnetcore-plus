using System;
using System.Collections.Generic;
using HealthCheck.AspNetCore.Plus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Plugins.Hangfire
{
    [FileDataSourceDiscriminator(Type)]
    public class HangfireHealthCheckItem : HealthCheckItem
    {
        public HangfireHealthCheckItem()
        {
            this.Name = Type;
            this.Tags = new List<string>() {"JobScheduler"};
        }
        
        public int? UnHealthyMinimumFailedCount { get; set; }
        public int? DegradedMinimumFailedCount { get; set; }
        public int? DegradedMinimumAvailableServers { get; set; }

        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        public const string Type = "Hangfire";
        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddHangfirePlus(o =>
            {
                o.DegradedMinimumAvailableServers = DegradedMinimumAvailableServers;
                o.DegradedMinimumFailedCount = DegradedMinimumFailedCount;
                o.UnHealthyMinimumFailedCount = UnHealthyMinimumFailedCount;
            }, Name, FailureStatus, Tags, Timeout);
        }

    }
}