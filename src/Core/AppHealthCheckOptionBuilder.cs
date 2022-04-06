using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthCheck.AspNetCore.Plus.DataSources;
using HealthCheck.AspNetCore.Plus.Models;
using HealthCheck.AspNetCore.Plus.Models.HealthCheckItems;
using HealthChecks.UI.Client;
using HealthChecks.UI.Configuration;
using JsonSubTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace HealthCheck.AspNetCore.Plus
{
    public static class AppHealthCheckOptionBuilder
    {
        public static AppHealthCheckBuilderOptions AddHealthCheckPlus(this IServiceCollection services)
        {
            return new AppHealthCheckBuilderOptions()
            {
                Options = new AppHealthCheckOptions()
                {
                    AddHealthEndpointPerHealthCheckItem = false,
                    AddHealthEndpointPerHealthCheckTag = false,
                    AddUIPerHealthCheckItem = false,
                    AddUIPerHealthCheckTag = false,
                    Services = services,
                },
                Services = services
            };
        }
        
        public static AppHealthCheckBuilderOptions CreateHealthEndpointPerTag(this AppHealthCheckBuilderOptions builder)
        {
            builder.Options.AddHealthEndpointPerHealthCheckTag = true;
            return builder;
        }

        public static AppHealthCheckBuilderOptions CreateHealthEndpointPerHealthCheckItem(this AppHealthCheckBuilderOptions builder)
        {
            builder.Options.AddHealthEndpointPerHealthCheckItem = true;
            return builder;
        }

        public static AppHealthCheckBuilderOptions CreateUIPerTag(this AppHealthCheckBuilderOptions builder)
        {
            builder.Options.AddUi = true;
            builder.Options.AddUIPerHealthCheckTag = true;
            return builder;
        }

        public static AppHealthCheckBuilderOptions CreateUIPerHealthCheckItem(this AppHealthCheckBuilderOptions builder)
        {
            builder.Options.AddUi = true;
            builder.Options.AddUIPerHealthCheckItem = true;
            return builder;
        }

        public static AppHealthCheckBuilderOptions AddFileDataSource(this AppHealthCheckBuilderOptions builder, Action<FileAppHealthCheckDataSource> configure)
        {
            var fileAppHealthCheckDataSource = new FileAppHealthCheckDataSource();
            builder.Options.DataSources.Add(fileAppHealthCheckDataSource);
            configure(fileAppHealthCheckDataSource);
            return builder;
        }

        public static AppHealthCheckBuilderOptions AddJsonDiscriminator<T>(this AppHealthCheckBuilderOptions builder, string discriminator) where T : HealthCheckItem
        {
            var fileDataSource = builder.Options.DataSources.OfType<FileAppHealthCheckDataSource>().FirstOrDefault();
            if (fileDataSource == null)
                throw new InvalidOperationException("file dataSource must be set before calling this method");
            
            fileDataSource.AddHealthCheckItemDiscriminator<T>(discriminator);
            return builder;
        }

        public static AppHealthCheckBuilderOptions AddInlineCodeDataSource(this AppHealthCheckBuilderOptions builder, string name, Func<HealthCheckContext, HealthCheckResult> customFunction, string groupName = "Default", HealthStatus? failureStatus = null, string[] tags = null, TimeSpan? timeout = null)
        {
            if (customFunction == null)
                throw new ArgumentNullException(nameof(customFunction));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var c = new CustomCodeHealthCheckItem()
            {
                Group = groupName, Name = name,
                FailureStatus = failureStatus ?? HealthStatus.Unhealthy,
                Timeout = timeout,
                Tags = tags,
                HealthCheckFunction = customFunction
            };
            builder.Options.DataSources.Add(new CustomCodeDataSource(c));
            return builder;
        }

        
        public static AppHealthCheckBuilderOptions AddHealthCheckItemDataSource<T>(this AppHealthCheckBuilderOptions builder, T healthCheckItem) where T : HealthCheckItem
        {
            if (healthCheckItem == null)
                throw new ArgumentNullException(nameof(healthCheckItem));
            
            builder.Options.DataSources.Add(new HealthCheckItemDataSource<T>(healthCheckItem));
            return builder;
        }

        public static AppHealthCheckBuilderOptions SetBasePath(this AppHealthCheckBuilderOptions builder, string basePath)
        {
            builder.Options.BasePath = basePath;
            return builder;
        }

        public static AppHealthCheckBuilderOptions AddHealthCheckUi(this AppHealthCheckBuilderOptions builder)
        {
            builder.Options.AddUi = true;
            return builder;
        }

        public static AppHealthCheckBuilderOptions ConfigureHealthCheck(this AppHealthCheckBuilderOptions builder, Action<List<HealthCheckItem>, HealthChecks.UI.Configuration.Settings> action)
        {
            builder.Options.HealthCheckUiBuildOptions = action;
            return builder;
        }

        public static AppHealthCheckBuilderOptions ConfigureHealthCheckUi(this AppHealthCheckBuilderOptions builder, Action<HealthChecksUIBuilder> action)
        {
            builder.Options.CustomizeHealthCheckUi = action;
            return builder;
        }

        public static AppHealthCheckOptions Build(this AppHealthCheckBuilderOptions builder)
        {
            var options = builder.Options;
            var healthCheckItems = options.DataSources.SelectMany(x => x.Retrieve()).ToList();
            if (options.AddUi)
            {
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
                    
                        foreach (var group in healthCheckItems.Select(x => x.Group).Distinct())
                        {
                            setup.AddHealthCheckEndpoint(group, $"{options.BasePath}/{group}");
                        }
                    
                        if (builder.Options.AddUIPerHealthCheckItem)
                        {
                            foreach (var item in healthCheckItems)
                                setup.AddHealthCheckEndpoint(item.Name, $"{options.BasePath}/_internals/{item.Name}");
                        }

                        if (builder.Options.AddUIPerHealthCheckTag)
                        {
                            var tags = healthCheckItems.SelectMany(x => x.Tags).Distinct();
                            foreach (var tag in tags)
                                setup.AddHealthCheckEndpoint(tag, $"{options.BasePath}/{tag}");
                        }

                        builder.Options.HealthCheckUiBuildOptions?.Invoke(healthCheckItems, setup);
                    });
                builder.Options.CustomizeHealthCheckUi?.Invoke(healthCheckUiBuilder);
            }
            builder.Services.AddHealthChecks(options);
            builder.Services.AddSingleton<AppHealthCheckOptions>(options);
            return builder.Options;
        }
        
        private static IHealthChecksBuilder AddHealthChecks(this IServiceCollection services, AppHealthCheckOptions options)
        {
            var healthChecksBuilder = services.AddHealthChecks();
            var healthCheckItems = options.DataSources.SelectMany(x => x.Retrieve()).ToList();
            
            foreach (var itemsByGroup in healthCheckItems.GroupBy(x => x.Name))
            {
                foreach (var item in itemsByGroup)
                {
                    try
                    {
                        var itemTags = new List<string>(item.Tags);
                        itemTags.Add(itemsByGroup.Key);
                        item.Tags = itemTags;
                        item.BuildHealthCheck(healthChecksBuilder);
                    }
                    catch (Exception e)
                    {
                        Serilog.Log.Error(e,"Cannot initialize {@HealthCheck} due to initialization error", item);
                    }
                }
            }

            return healthChecksBuilder;
        }

        public static void MapAppHealthChecksEndpoints(this IEndpointRouteBuilder config,
            Action<Options> setup = null)
        {
            var options = config.ServiceProvider.GetService<AppHealthCheckOptions>();
            config.MapHealthChecks($"{options.BasePath}", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = GetResponseWriter(options)
            });
            var healthCheckItems = options.DataSources.SelectMany(x => x.Retrieve()).ToList();

            if (options.AddHealthEndpointPerHealthCheckItem)
            {
                foreach (var item in healthCheckItems)
                {
                    config.MapHealthChecks($"{options.BasePath}/_internals/{item.Name}", new HealthCheckOptions
                    {
                        Predicate = _ => _.Name == item.Name,
                        ResponseWriter = GetResponseWriter(options)
                    });
                }
            }

            if (options.AddHealthEndpointPerHealthCheckTag)
            {
                var tags = healthCheckItems.SelectMany(x => x.Tags).Distinct();
                foreach (var tag in tags)
                {
                    config.MapHealthChecks($"{options.BasePath}/{tag}", new HealthCheckOptions
                    {
                        Predicate = _ => _.Tags.Contains(tag),
                        ResponseWriter = GetResponseWriter(options)
                    });
                }
            }

            var groups = healthCheckItems.GroupBy(x => x.Group);
            foreach (var groupItem in groups)
            {
                config.MapHealthChecks($"{options.BasePath}/{groupItem.Key}", new HealthCheckOptions
                {
                    Predicate = _ => _.Tags.Contains(groupItem.Key),
                    ResponseWriter = GetResponseWriter(options)
                });
            }

            if (options.AddUi)
            {
                config.MapHealthChecksUI(setup ?? (op => { }));
            }
        }

        private static Func<HttpContext, HealthReport, Task> GetResponseWriter(AppHealthCheckOptions options)
        {
            if (options.AddUi)
                return UIResponseWriter.WriteHealthCheckUIResponse;

            return null;
        }
    }
}