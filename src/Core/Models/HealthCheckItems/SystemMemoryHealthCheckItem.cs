using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    [FileDataSourceDiscriminator(Type)]
    public class SystemMemoryHealthCheckItem : HealthCheckItem
    {
        public SystemMemoryHealthCheckItem()
        {
            this.Name = Type;
            this.Tags = new[] {"System", "Infrastructure"};
        }
        
        public const string Type = "SystemMemory";
        public int MaximumMemoryInMegaBytes { get; set; }
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        
        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddPrivateMemoryHealthCheck(MaximumMemoryInMegaBytes, Name, FailureStatus, Tags, Timeout);
        }
    }
}