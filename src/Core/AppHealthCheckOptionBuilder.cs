using System;
using System.Collections.Generic;
using System.Linq;
using HealthCheck.AspNetCore.Plus.Models;
using HealthCheck.AspNetCore.Plus.Models.HealthCheckItems;
using HealthChecks.UI.Client;
using JsonSubTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace HealthCheck.AspNetCore.Plus
{
    public class AppHealthCheckBuilderOptions
    {
        internal AppHealthCheckOptions Options { get; set; }
        internal IServiceCollection Services { get; set; }
    }
    
    public static class AppHealthCheckOptionBuilder
    {
        public static AppHealthCheckBuilderOptions AddAppHealthCheck(this IServiceCollection services)
        {
            return new AppHealthCheckBuilderOptions()
            {
                Options = new AppHealthCheckOptions()
                {
                    AddHealthCheckEndpointPerHealthCheckName = false,
                    AddHealthCheckEndpointPerHealthCheckTag = false,
                    AddHealthCheckUIPerHealthCheckName = false,
                    AddHealthCheckUIPerHealthCheckTag = false,
                    Services = services,
                },
                Services = services
            };
        }
        
        public static AppHealthCheckBuilderOptions CreateHealthEndpointPerTag(this AppHealthCheckBuilderOptions builder)
        {
            builder.Options.AddHealthCheckEndpointPerHealthCheckTag = true;
            return builder;
        }

        public static AppHealthCheckBuilderOptions CreateHealthEndpointPerName(this AppHealthCheckBuilderOptions builder)
        {
            builder.Options.AddHealthCheckEndpointPerHealthCheckName = true;
            return builder;
        }

        public static AppHealthCheckBuilderOptions CreateUIPerTag(this AppHealthCheckBuilderOptions builder)
        {
            builder.Options.AddHealthCheckUIPerHealthCheckTag = true;
            return builder;
        }

        public static AppHealthCheckBuilderOptions CreateUIPerName(this AppHealthCheckBuilderOptions builder)
        {
            builder.Options.AddHealthCheckUIPerHealthCheckName = true;
            return builder;
        }
        
        public static AppHealthCheckBuilderOptions SetFileName(this AppHealthCheckBuilderOptions builder, string fileName)
        {
            builder.Options.HealthCheckSettingsFile = fileName;
            return builder;
        }

        public static AppHealthCheckBuilderOptions SetAppHealthCheckConfiguration(this AppHealthCheckBuilderOptions builder, Action<AppHealthCheckConfiuration, HealthChecks.UI.Configuration.Settings> action)
        {
            builder.Options.HealthCheckUiBuildOptions = action;
            return builder;
        }

        public static AppHealthCheckBuilderOptions CustomizeHealthCheckUiSettings(this AppHealthCheckBuilderOptions builder, Action<HealthChecksUIBuilder> action)
        {
            builder.Options.CustomizeHealthCheckUi = action;
            return builder;
        }

        public static AppHealthCheckBuilderOptions AddHealthCheckItemDiscriminator<T>(this AppHealthCheckBuilderOptions builder, string discriminator)
            where T : HealthCheckItem
        {
            builder.Options.AddHealthCheckItemDiscriminator<T>(discriminator);
            return builder;
        }


        public static void Build(this AppHealthCheckBuilderOptions builder)
        {
            var healthCheckConfiguration = builder.Services.ConfigureAndGetHealthCheckConfiguration(builder.Options);
            builder.Services.AddSingleton(builder);
            var healthCheckUiBuilder = builder.Services
                .AddHealthChecksUI(setupSettings: setup =>
                {
               
                    //Configures the UI to poll for healthchecks updates every 5 seconds, default is every 10 seconds
                    //setup.SetEvaluationTimeInSeconds(5); 

                    //Only one active request will be executed at a time.
                    //All the excedent requests will result in 429 (Too many requests)
                    //setup.SetApiMaxActiveRequests(1);

                    // Set the maximum history entries by endpoint that will be served by the UI api middleware
                    //setup.MaximumHistoryEntriesPerEndpoint(50);
                    if (healthCheckConfiguration.Categories.Any())
                    {
                        foreach (var category in healthCheckConfiguration.Categories)
                        {
                            foreach (var item in category.Value)
                                setup.AddHealthCheckEndpoint(item.Name, $"/healthz/{category.Key}/{item.Name}");
                        }
                    }
                    
                    if (builder.Options.AddHealthCheckUIPerHealthCheckName)
                    {
                        foreach (var item in healthCheckConfiguration.HealthChecks)
                            setup.AddHealthCheckEndpoint(item.Name, $"/healthz/internals/{item.Name}");
                    }

                    if (builder.Options.AddHealthCheckUIPerHealthCheckTag)
                    {
                        var tags = healthCheckConfiguration.HealthChecks.SelectMany(x => x.Tags).Distinct();
                        foreach (var tag in tags)
                            setup.AddHealthCheckEndpoint(tag, $"/healthz/components/{tag}");
                    }

                    builder.Options.HealthCheckUiBuildOptions?.Invoke(healthCheckConfiguration, setup);
                });
            builder.Options.CustomizeHealthCheckUi?.Invoke(healthCheckUiBuilder);
            builder.Services.AddHealthChecks(healthCheckConfiguration);
        }
        
        private static AppHealthCheckConfiuration ConfigureAndGetHealthCheckConfiguration(this IServiceCollection services, AppHealthCheckOptions options)
        {
            services.AddSingleton(options);
            var json = System.IO.File.ReadAllText(options.HealthCheckSettingsFile);
            var serializerSettings = new JsonSerializerSettings();
            var jsonSubtypesConverterBuilder = JsonSubtypesConverterBuilder
                .Of(typeof(HealthCheckItem), "Type")
                .SerializeDiscriminatorProperty()
                .RegisterSubtype(typeof(DnsResolutionHealthCheckItem), "DNS")
                .RegisterSubtype(typeof(FtpHealthCheckItem), "FTP")
                .RegisterSubtype(typeof(IdentityServerHealthCheckItem), "IdentityServer")
                .RegisterSubtype(typeof(KafkaHealthCheckItem), "Kafka")
                .RegisterSubtype(typeof(PingHealthCheckItem), "Ping")
                .RegisterSubtype(typeof(RedisHealthCheckItem), "Redis")
                .RegisterSubtype(typeof(HangfireHealthCheckItem), "Hangfire")
                .RegisterSubtype(typeof(SftpHealthCheckItem), "SFTP")
                .RegisterSubtype(typeof(SmtpHealthCheckItem), "SMTP")
                .RegisterSubtype(typeof(SslCertificateHealthCheckItem), "SSL")
                .RegisterSubtype(typeof(TcpHealthCheckItem), "TCP")
                .RegisterSubtype(typeof(HttpHealthCheckItem), "HTTP")
                .RegisterSubtype(typeof(ProcessMemoryHealthCheckItem), "ProcessMemory")
                .RegisterSubtype(typeof(SystemDiskHealthCheckItem), "SystemDisk")
                .RegisterSubtype(typeof(SystemMemoryHealthCheckItem), "SystemMemory")
                .RegisterSubtype(typeof(SystemProcessHealthCheckItem), "SystemProcess");
            foreach (var typeDiscriminator in options.GetHealthCheckItemDiscriminators())
            {
                jsonSubtypesConverterBuilder.RegisterSubtype(typeDiscriminator.Key, typeDiscriminator.Value);
            }
            serializerSettings.Converters.Add(jsonSubtypesConverterBuilder.Build());

            var settings = JsonConvert.DeserializeObject<AppHealthCheckConfiuration>(json, serializerSettings);
            services.AddSingleton(settings);
            return settings;
        }
        
        private static IHealthChecksBuilder AddHealthChecks(this IServiceCollection services, AppHealthCheckConfiuration confiuration)
        {
            var healthChecksBuilder = services.AddHealthChecks();
            foreach (var healthCheckItem in confiuration.HealthChecks)
            {
                try
                {
                    healthCheckItem.BuildHealthCheck(healthChecksBuilder);
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e,"Cannot initialize {@HealthCheck} due to initialization error", healthCheckItem);
                }
            }
            
            if (confiuration.Categories.Any())
            {
                foreach (var category in confiuration.Categories)
                {
                    foreach (var item in category.Value)
                    {
                        try
                        {
                            var itemTags = new List<string>(item.Tags);
                            itemTags.Add(category.Key);
                            item.Tags = itemTags;
                            item.BuildHealthCheck(healthChecksBuilder);
                        }
                        catch (Exception e)
                        {
                            Serilog.Log.Error(e,"Cannot initialize {@HealthCheck} due to initialization error", item);
                        }
                    }
                }
            }


            return healthChecksBuilder;
        }

        public static void MapAppHealthChecksEndpoints(this IEndpointRouteBuilder config)
        {
            config.MapHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            var appHealthCheckConfiuration =  config.ServiceProvider.GetRequiredService<AppHealthCheckConfiuration>();
            var options =  config.ServiceProvider.GetRequiredService<AppHealthCheckOptions>();
            if (options.AddHealthCheckEndpointPerHealthCheckName)
            {
                foreach (var item in appHealthCheckConfiuration.HealthChecks)
                {
                    config.MapHealthChecks($"/healthz/internals/{item.Name}", new HealthCheckOptions
                    {
                        Predicate = _ => _.Name == item.Name,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                }
            }

            if (options.AddHealthCheckEndpointPerHealthCheckTag)
            {
                var tags = appHealthCheckConfiuration.HealthChecks.SelectMany(x => x.Tags).Distinct();
                foreach (var tag in tags)
                {
                    config.MapHealthChecks($"/healthz/components/{tag}", new HealthCheckOptions
                    {
                        Predicate = _ => _.Tags.Contains(tag),
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                }
            }

            var categories = appHealthCheckConfiuration.Categories;
            foreach (var category in categories)
            {
                config.MapHealthChecks($"/healthz/{category.Key}", new HealthCheckOptions
                {
                    Predicate = _ => _.Tags.Contains(category.Key),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            }

            config.MapHealthChecksUI(setup =>
            {
                //Customize Health check ui using custom css
                //setup.AddCustomStylesheet("dotnet.css");
            });
        }

    }
}