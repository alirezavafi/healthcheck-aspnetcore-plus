using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public class DnsResolutionHealthCheckItem : HealthCheckItem
    {
        public DnsResolutionHealthCheckItem()
        {
            this.Tags = new[] {"Network", "Infrastructure"};
        }
        public string Host { get; set;}
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;

        public sealed override string Type => "DNS";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddDnsResolveHealthCheck(o => o.ResolveHost(Host), Name, FailureStatus, Tags);
        }
    }
}