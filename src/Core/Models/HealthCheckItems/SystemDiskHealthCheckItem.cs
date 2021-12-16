using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public class SystemDiskHealthCheckItem : HealthCheckItem
    {
        public SystemDiskHealthCheckItem()
        {
            this.Name = this.Type;
            this.Tags = new[] {"System", "Infrastructure"};
        }
        
        public sealed override string Type => "SystemDisk";
        public string DriveName { get; set; }
        public int MinimumRequiredFreeSpace { get; set; }
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        
        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddDiskStorageHealthCheck(o => o.AddDrive(DriveName, MinimumRequiredFreeSpace), Name, FailureStatus, Tags, Timeout);
        }
    }
}