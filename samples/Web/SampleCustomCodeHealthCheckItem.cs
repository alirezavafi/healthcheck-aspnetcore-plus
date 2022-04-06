using System;
using System.Threading;
using System.Threading.Tasks;
using HealthCheck.AspNetCore.Plus.Models;
using HealthCheck.AspNetCore.Plus.Models.HealthCheckItems;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Samples.Web
{
    public class SampleCustomCodeHealthCheckItem : CustomCodeHealthCheckItem
    {
        protected override Task<HealthCheckResult> HealthCheckFunction(IServiceProvider serviceProvider, HealthCheckContext context,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy));
        }
    }
}