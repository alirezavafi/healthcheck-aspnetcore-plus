using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public class CustomCodeDelegateHealthCheckItem : CustomCodeHealthCheckItem
    {
        public const string Type = "CustomCodeDelegate";
        public Func<IServiceProvider, HealthCheckContext, CancellationToken, Task<HealthCheckResult>> HealthCheckDelegate { get; set; }

        protected override Task<HealthCheckResult> HealthCheckFunction(IServiceProvider serviceProvider, HealthCheckContext context,
            CancellationToken cancellationToken)
        {
            if (HealthCheckDelegate == null)
                throw new InvalidOperationException("HealthCheckDelegate is not set");
            return HealthCheckDelegate(serviceProvider, context, cancellationToken);
        }
    }
}