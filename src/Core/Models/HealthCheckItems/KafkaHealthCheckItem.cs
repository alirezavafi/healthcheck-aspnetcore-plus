using System;
using AutoMapper;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck.AspNetCore.Plus.Models.HealthCheckItems
{
    public class KafkaHealthCheckItem : HealthCheckItem
    {
        public KafkaHealthCheckItem()
        {
            this.Name = this.Type;
            this.Tags = new[] {"Messaging"};
        }
        public ProducerConfig Configuration { get; set; }
        public string Topic { get; set; } = null;
        public HealthStatus FailureStatus { get; set; } = HealthStatus.Unhealthy;
        public TimeSpan? Timeout { get; set;}
        
        public sealed override string Type => "Kafka";

        public override void BuildHealthCheck(IHealthChecksBuilder builder)
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<ProducerConfig, ProducerConfig>());
            var mapper = mapperConfig.CreateMapper();
            builder.AddKafka(o => mapper.Map(Configuration, o), Topic, Name, FailureStatus, Tags, Timeout);
        }
    }
}