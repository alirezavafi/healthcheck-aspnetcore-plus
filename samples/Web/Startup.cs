using HealthCheck.AspNetCore.Plus.Plugins.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HealthCheck.AspNetCore.Plus.Samples.Web
{
    public class Startup
    {
        private AppHealthCheckOptions _appHealthCheckOptions;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _appHealthCheckOptions = services.AddAppHealthCheck()
                .AddFileDataSource(o => o.SetFileName("healthz.json"))
                .AddMySqlPlugin()
                .SetAppHealthCheckConfiguration((s, o) => o.SetEvaluationTimeInSeconds(1))
                .CustomizeHealthCheckUiSettings(p => p.AddInMemoryStorage()) ////healthChecks.UiBuilder.AddMySqlStorage("connectionString")
                .CreateHealthEndpointPerTag()
                .CreateHealthEndpointPerName()
                .CreateUIPerName()
                .Build();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(config => { config.MapAppHealthChecksEndpoints(_appHealthCheckOptions); });
        }
    }
}