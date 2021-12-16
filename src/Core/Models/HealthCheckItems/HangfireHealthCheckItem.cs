﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public class HangfireHealthCheckItem : HealthCheckItem
    {
        public HangfireHealthCheckItem()
        {
            this.Name = this.Type;
            this.Tags = new List<string>() {"JobScheduler", "Service"};
        }
        
        public int? MinimumAvailableServers { get; set; }
        public int? MaximumFailedJobs { get; set; }
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        public sealed override string Type => "Hangfire";
        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddHangfire(o =>
            {
                o.MaximumJobsFailed = MaximumFailedJobs;
                o.MinimumAvailableServers = MinimumAvailableServers;
            }, Name, FailureStatus, Tags, Timeout);
        }

    }
}