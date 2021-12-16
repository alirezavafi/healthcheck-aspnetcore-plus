using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public class ProcessMemoryHealthCheckItem : HealthCheckItem
    {
        public ProcessMemoryHealthCheckItem()
        {
            this.Name = this.Type;
            this.Tags = new[] {"System", "Infrastructure"};
        }
        
        public sealed override string Type => "ProcessMemory";
        public int MaximumMemoryInMegaBytes { get; set; }
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        
        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddProcessAllocatedMemoryHealthCheck(MaximumMemoryInMegaBytes, Name, FailureStatus, Tags, Timeout);
        }
    }
}