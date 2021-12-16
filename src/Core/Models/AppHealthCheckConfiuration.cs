using System.Collections.Generic;

namespace HealthCheck.AspNetCore.Plus.Models
{
    public class AppHealthCheckConfiuration
    {
        public IDictionary<string, List<HealthCheckItem>> Categories { get; set; } = new Dictionary<string, List<HealthCheckItem>>();
        public IList<HealthCheckItem> HealthChecks { get; set; } = new List<HealthCheckItem>();
    }
}