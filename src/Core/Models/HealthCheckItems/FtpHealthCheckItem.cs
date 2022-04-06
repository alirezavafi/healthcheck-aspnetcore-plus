using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    [FileDataSourceDiscriminator(Type)]
    public class FtpHealthCheckItem : HealthCheckItem
    {
        public FtpHealthCheckItem()
        {
            this.Name = Type;
            this.Tags = new[] {"Network", "Infrastructure"};
        }
        
        public string Host { get; set;}
        public int Port { get; set; } = 443;
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        public string UserDomainName { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public bool CreateFile { get; set; }
        
        public const string Type = "FTP";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddFtpHealthCheck(
               o => o.AddHost(Host, CreateFile,
                  new NetworkCredential(Username, Password, UserDomainName)), Name,
               FailureStatus, Tags, Timeout);
        }
    }
}