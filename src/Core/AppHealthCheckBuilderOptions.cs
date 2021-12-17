using Microsoft.Extensions.DependencyInjection;

namespace HealthCheck.AspNetCore.Plus
{
    public class AppHealthCheckBuilderOptions
    {
        internal AppHealthCheckOptions Options { get; set; }
        internal IServiceCollection Services { get; set; }
    }
}