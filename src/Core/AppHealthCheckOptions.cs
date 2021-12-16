using System;
using System.Collections.Generic;
using HealthCheck.AspNetCore.Plus.Models;
using Microsoft.Extensions.DependencyInjection;

namespace HealthCheck.AspNetCore.Plus
{
    public class AppHealthCheckOptions
    {
        private readonly Dictionary<Type, string> _customTypeDiscriminators = new Dictionary<Type, string>();

        public void AddHealthCheckItemDiscriminator<T>(string discriminator) where T : HealthCheckItem =>
            _customTypeDiscriminators.Add(typeof(T), discriminator);

        public Dictionary<Type, string> GetHealthCheckItemDiscriminators() => _customTypeDiscriminators;

        public Action<AppHealthCheckConfiuration, HealthChecks.UI.Configuration.Settings> HealthCheckUiBuildOptions
        {
            get;
            set;
        }

        public bool AddHealthCheckEndpointPerHealthCheckTag { get; set; } = true;
        public bool AddHealthCheckUIPerHealthCheckTag { get; set; } = true;
        public bool AddHealthCheckEndpointPerHealthCheckName { get; set; } = false;
        public bool AddHealthCheckUIPerHealthCheckName { get; set; } = false;
        public string HealthCheckSettingsFile { get; set; } = "healthz.json";
        public IServiceCollection Services { get; set; }
        public Action<HealthChecksUIBuilder> CustomizeHealthCheckUi { get; set; }
    }
}