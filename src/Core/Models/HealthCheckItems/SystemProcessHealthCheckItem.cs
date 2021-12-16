using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public class SystemProcessHealthCheckItem : HealthCheckItem
    {
        public SystemProcessHealthCheckItem()
        {
            this.Name = this.Type;
            this.Tags = new[] {"System", "Infrastructure", "Service"};
        }
        
        public sealed override string Type => "SystemProcess";
        public string ProcessName { get; set; }
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddProcessHealthCheck(ProcessName, processes => processes.Any(x => x.ProcessName == ProcessName),
                Name, FailureStatus, Tags, Timeout);
        }
    }
}