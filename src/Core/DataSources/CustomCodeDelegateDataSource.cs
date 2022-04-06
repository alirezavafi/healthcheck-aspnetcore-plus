using System.Collections.Generic;
using HealthCheck.AspNetCore.Plus.Models;
using HealthCheck.AspNetCore.Plus.Models.HealthCheckItems;

namespace HealthCheck.AspNetCore.Plus.DataSources
{
    public class CustomCodeDelegateDataSource : IAppHealthCheckDataSource
    {
        public CustomCodeDelegateHealthCheckItem Settings { get; }
        public CustomCodeDelegateDataSource() { }
        public CustomCodeDelegateDataSource(CustomCodeDelegateHealthCheckItem settings)
        {
            Settings = settings;
        }

        public List<HealthCheckItem> Retrieve()
        {
            return new List<HealthCheckItem>() { Settings };
        }
    }
}