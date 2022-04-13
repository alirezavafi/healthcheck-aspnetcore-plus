using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    [FileDataSourceDiscriminator(Type)]
    public class OpenIdConnectHealthCheckItem : HealthCheckItem
    {
        public OpenIdConnectHealthCheckItem()
        {
            this.Name = Type;
            this.Tags = new[] {"oidc"};
        }
        public Uri Url { get; set;}
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        
        public const string Type = "OpenIdConnect";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddIdentityServer(Url, Name, FailureStatus, Tags, Timeout);
        }
    }
}