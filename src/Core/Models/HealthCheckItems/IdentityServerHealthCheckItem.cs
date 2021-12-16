using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public class IdentityServerHealthCheckItem : HealthCheckItem
    {
        public IdentityServerHealthCheckItem()
        {
            this.Name = this.Type;
            this.Tags = new[] {"WebServer", "IdentityServer"};
        }
        public Uri Url { get; set;}
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        
        public sealed override string Type => "IdentityServer";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddIdentityServer(Url, Name, FailureStatus, Tags, Timeout);
        }
    }
}