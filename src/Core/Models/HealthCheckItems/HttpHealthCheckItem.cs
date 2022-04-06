using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    [FileDataSourceDiscriminator(Type)]
    public class HttpHealthCheckItem : HealthCheckItem
    {
        public HttpHealthCheckItem()
        {
            this.Name = Type;
            this.Tags = new[] {"Web"};
        }
        
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public Dictionary<string, string> HttpHeaders { get; set; } = new Dictionary<string, string>();
        public int MinExpectedHttpCode { get; set; } = 200;
        public int MaxExpectedHttpCode { get; set; } = 399;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        public Uri Uri { get; set; }
        public HttpMethod HttpMethod { get; set; }
        
        public const string Type = "Url";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.AddUrlGroup(o =>
            {
                o.AddUri(this.Uri, options =>
                {
                    options.UseHttpMethod(this.HttpMethod);
                    o.ExpectHttpCodes(this.MinExpectedHttpCode, this.MaxExpectedHttpCode);
                    foreach (var header in HttpHeaders)
                        options.AddCustomHeader(header.Key, header.Value);
                    o.UseTimeout(this.Timeout);
                });
            }, Name, FailureStatus, Tags, Timeout);
        }
    }
}