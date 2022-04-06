using HealthCheck.AspNetCore.Plus.Models.HealthCheckItems;
using HealthCheck.AspNetCore.Plus.Plugins.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace HealthCheck.AspNetCore.Plus.Samples.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthCheckPlus()
                .AddMySqlPlugin()
                .ConfigureHealthCheck((s, o) => o.SetEvaluationTimeInSeconds(1))
                .ConfigureHealthCheckUi(p => p.AddInMemoryStorage()) ////healthChecks.UiBuilder.AddMySqlStorage("connectionString")
                .CreateHealthEndpointPerTag()
                .CreateHealthEndpointPerHealthCheckItem()
                .CreateUIPerHealthCheckItem()
                .AddFileDataSource(o => o.SetFileName("healthz.json"))
                .AddInlineCodeDataSource("Self", context => new HealthCheckResult(HealthStatus.Healthy), groupName: "Api", tags: new[] {"live", "ready", "api"})
                .AddHealthCheckItemDataSource(new CustomHealthCheckItem())
                .AddHealthCheckItemDataSource(new PingHealthCheckItem()
                {
                    Group = "Availability",
                    Name = "InternetConnectivity",
                    Tags = new []{ "live", "network"},
                    Host = "yahoo.com"
                })
                .Build();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(config => { config.MapAppHealthChecksEndpoints(o =>
            {
                o.UIPath = "/health/ui";
            }); });
        }
    }
}