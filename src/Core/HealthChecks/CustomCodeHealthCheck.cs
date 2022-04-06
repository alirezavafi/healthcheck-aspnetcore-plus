using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public class CustomCodeHealthCheck : IHealthCheck
    {
        private readonly Func<IServiceProvider, HealthCheckContext, CancellationToken, Task<HealthCheckResult>> _healthCheckContext;
        public IServiceProvider ServiceProvider { get; }

        public CustomCodeHealthCheck(IServiceProvider serviceProvider, Func<IServiceProvider, HealthCheckContext, CancellationToken, Task<HealthCheckResult>> healthCheckContext)
        {
            _healthCheckContext = healthCheckContext;
            ServiceProvider = serviceProvider;
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            return _healthCheckContext(ServiceProvider, context, cancellationToken);
        }
    }
}