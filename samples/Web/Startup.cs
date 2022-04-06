using System.Threading.Tasks;
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
                .AddMySqlFileDataSourceType()
                .ConfigureHealthCheck((s, o) => o.SetEvaluationTimeInSeconds(1))
                .ConfigureHealthCheckUi(p => p.AddInMemoryStorage()) ////healthChecks.UiBuilder.AddMySqlStorage("connectionString")
                .CreateHealthEndpointPerTag()
                .CreateHealthEndpointPerHealthCheckItem()
                .CreateUIPerHealthCheckItem()
                .AddHealthCheckFileDataSource(o => o.SetFileName("healthz.json"))
                .AddInlineCodeHealthCheck("Self", (s, c, t) => Task.FromResult(new HealthCheckResult(HealthStatus.Healthy)), groupName: "Api", tags: new[] {"live", "ready", "api"})
                .AddHealthCheckItem(new SampleCustomCodeHealthCheckItem()
                {
                    Name = "Custom",
                    Group = "Custom",
                    Tags = new []{ "live", "core"}
                })
                .AddHealthCheckItem(new PingHealthCheckItem()
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
            app.UseEndpoints(e => { e.MapAppHealthChecksEndpoints(o =>
            {
                //o.UIPath = "/healthz/ui";
            }); });
        }
    }
}