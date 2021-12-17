using System.Collections.Generic;

namespace HealthCheck.AspNetCore.Plus.Models
{
    public class AppHealthCheckDataSource
    {
        public IDictionary<string, List<HealthCheckItem>> Groups { get; set; } =
            new Dictionary<string, List<HealthCheckItem>>();

        public IList<HealthCheckItem> HealthChecks { get; set; } = new List<HealthCheckItem>();
    }
}