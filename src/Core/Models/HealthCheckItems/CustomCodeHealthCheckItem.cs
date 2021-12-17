using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public class CustomCodeHealthCheckItem : HealthCheckItem, IHealthCheck
    {
        public CustomCodeHealthCheckItem() => this.Name = this.Type;
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        public sealed override string Type => "CustomCode";
        public Func<HealthCheckContext, HealthCheckResult> HealthCheckFunction { get; set; }
        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddCheck(Name, this, FailureStatus, Tags, Timeout);
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var healthCheckFunction = this.HealthCheckFunction(context);
            return Task.FromResult(healthCheckFunction);
        }
    }
}