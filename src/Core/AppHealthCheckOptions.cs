using System;
using System.Collections.Generic;
using HealthCheck.AspNetCore.Plus.DataSources;
using HealthCheck.AspNetCore.Plus.Models;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCheck.AspNetCore.Plus
{
    public class AppHealthCheckOptions
    {

        public Action<List<HealthCheckItem>, HealthChecks.UI.Configuration.Settings> HealthCheckUiBuildOptions
        {
            get;
            set;
        }

        public bool AddHealthEndpointPerHealthCheckTag { get; set; } = true;
        public bool AddHealthEndpointPerHealthCheckItem { get; set; } = false;
        public bool AddUi { get; set; } = false;
        public bool AddUIPerHealthCheckTag { get; set; } = true;
        public bool AddUIPerHealthCheckItem { get; set; } = false;
        public IServiceCollection Services { get; set; }
        public Action<HealthChecksUIBuilder> CustomizeHealthCheckUi { get; set; }
        public IList<IAppHealthCheckDataSource> DataSources { get; } = new List<IAppHealthCheckDataSource>();
        public string BasePath { get; set; } = "/healthz";
    }
}