using System.Collections.Generic;
using HealthCheck.AspNetCore.Plus.Models;
using HealthCheck.AspNetCore.Plus.Models.HealthCheckItems;

namespace HealthCheck.AspNetCore.Plus.DataSources
{
    public class CustomCodeDataSource : IAppHealthCheckDataSource
    {
        public CustomCodeHealthCheckItem Settings { get; }
        public CustomCodeDataSource() { }
        public CustomCodeDataSource(CustomCodeHealthCheckItem settings)
        {
            Settings = settings;
        }

        public List<HealthCheckItem> Retrieve()
        {
            return new List<HealthCheckItem>() { Settings };
        }
    }
}