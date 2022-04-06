using HealthCheck.AspNetCore.Plus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Samples.Web
{
    public class CustomHealthCheckItem : HealthCheckItem
    {
        public override string Type { get; } = "Custom";
        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddDnsResolveHealthCheck(o => o.ResolveHost("google.com"), Name, HealthStatus.Unhealthy, Tags);
        }
    }
}