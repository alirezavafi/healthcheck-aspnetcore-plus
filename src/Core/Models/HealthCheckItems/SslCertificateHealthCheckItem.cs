using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public class SslCertificateHealthCheckItem : HealthCheckItem
    {
        public SslCertificateHealthCheckItem()
        {
            this.Name = this.Type;
            this.Tags = new[] {"Network", "Infrastructure", "Web"};
        }
        
        public string Host { get; set;}
        public int Port { get; set; } = 443;
        public int? RemainDaysToHealthFailure { get; set; } = 5;
        public int? RemainDaysToHealthDegraded { get; set; } = 30;
        public TimeSpan? Timeout { get; set;}

        public sealed override string Type => "SSLCertificate";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            if (RemainDaysToHealthFailure != null)
            {
                builder.AddSslHealthCheck(
                  o => o.AddHost(Host, Port, RemainDaysToHealthFailure.Value), Name,
                  failureStatus: HealthStatus.Unhealthy, Tags, Timeout);
            }
            
            if (RemainDaysToHealthDegraded != null)
            {
                builder.AddSslHealthCheck(
                  o => o.AddHost(Host, Port, RemainDaysToHealthDegraded.Value), Name,
                  failureStatus: HealthStatus.Degraded, Tags, Timeout);
            }
        }
    }
}