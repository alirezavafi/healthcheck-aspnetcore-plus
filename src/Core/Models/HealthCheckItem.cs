using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCheck.AspNetCore.Plus.Models
{
    public abstract class HealthCheckItem
    {
        public string Name { get; set;}
        public string Group { get; set; } = "Default";
        public IEnumerable<string> Tags { get; set; } = new List<string>();
        public abstract string Type { get;}
        public abstract void BuildHealthCheck(IHealthChecksBuilder builder);
    }
}