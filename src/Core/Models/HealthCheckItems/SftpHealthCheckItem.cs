using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    [FileDataSourceDiscriminator(Type)]
    public class SftpHealthCheckItem : HealthCheckItem
    {
        public SftpHealthCheckItem()
        {
            this.Name = Type;
            this.Tags = new[] {"Network", "Infrastructure"};
        }
        
        public string Host { get; set;}
        public int Port { get; set; } = 443;
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}

        public const string Type = "SFTP";
        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            //healthChecksBuilder.AddSftpHealthCheck(o => o.AddHost(new SftpConfiguration(new AuthenticationMet)))
        }
    }
}