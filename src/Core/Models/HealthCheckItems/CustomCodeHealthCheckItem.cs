using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public abstract class CustomCodeHealthCheckItem : HealthCheckItem
    {
        public CustomCodeHealthCheckItem() => this.Name = Type;
        public TimeSpan? Timeout { get; set;}
        public const string Type = "CustomCode";
        public sealed override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            builder.Add(new HealthCheckRegistration(
                Name,
                sp => new CustomCodeHealthCheck(sp.GetRequiredService<IServiceProvider>(), this.HealthCheckFunction),
                null,
                Tags, Timeout));
        }

        protected abstract Task<HealthCheckResult> HealthCheckFunction(IServiceProvider serviceProvider,
            HealthCheckContext context, CancellationToken cancellationToken);
    }
}