using System;
using HealthChecks.Network.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public class SmtpHealthCheckItem : HealthCheckItem
    {
        public SmtpHealthCheckItem()
        {
            this.Name = this.Type;
            this.Tags = new[] {"Network", "Infrastructure", "Email"};
        }
        
        public string Host { get; set;}
        public int Port { get; set; } = 443;
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        public bool AllowInvalidRemoteCertificates { get; set; } = false;
        public SmtpConnectionType ConnectionType { get; set; } = SmtpConnectionType.AUTO;
        public string Password { get; set; }
        public string Username { get; set; }
        
        public sealed override string Type { get; } = "SMTP";
        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddSmtpHealthCheck(o =>
            {
               o.Host = Host;
               o.Port = Port;
               o.AllowInvalidRemoteCertificates = AllowInvalidRemoteCertificates;
               o.ConnectionType = ConnectionType;
               o.LoginWith(Username, Password);
            }, Name, FailureStatus, Tags, Timeout);
        }
    }
}