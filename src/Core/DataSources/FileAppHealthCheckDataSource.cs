using System;
using System.Collections.Generic;
using System.Linq;
using HealthCheck.AspNetCore.Plus.Models;
using HealthCheck.AspNetCore.Plus.Models.HealthCheckItems;
using JsonSubTypes;
using Newtonsoft.Json;

namespace HealthCheck.AspNetCore.Plus.DataSources
{
    public class FileAppHealthCheckDataSource : IAppHealthCheckDataSource
    {
        private readonly Dictionary<Type, string> _customTypeDiscriminators = new Dictionary<Type, string>();

        public FileAppHealthCheckDataSource AddHealthCheckItemDiscriminator<T>(string discriminator) where T : HealthCheckItem
        {
            _customTypeDiscriminators.Add(typeof(T), discriminator);
            return this;
        }
        public FileAppHealthCheckDataSource SetFileName(string fileName)
        {
            this.FileName = fileName;
            return this;
        }
        public FileAppHealthCheckDataSource SetFileOptional()
        {
            this.IsOptional = true;
            return this;
        }

        private Dictionary<Type, string> GetHealthCheckItemDiscriminators() => _customTypeDiscriminators;
        public string FileName { get; set; }
        public bool IsOptional { get; set; }
        public FileAppHealthCheckDataSource() { }
        public FileAppHealthCheckDataSource(string fileName, bool isOptional)
        {
            FileName = fileName;
            IsOptional = isOptional;
        }

        public List<HealthCheckItem> Retrieve()
        {
            if (string.IsNullOrWhiteSpace(FileName) || !System.IO.File.Exists(FileName))
            {
                if (!IsOptional)
                {
                    //log and throw error
                    throw new InvalidOperationException($"File {FileName} invalid or does not exists");
                }

                return new List<HealthCheckItem>();
            }

            var json = System.IO.File.ReadAllText(FileName);
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
            foreach (var typeDiscriminator in GetHealthCheckItemDiscriminators())
            {
                jsonSubtypesConverterBuilder.RegisterSubtype(typeDiscriminator.Key, typeDiscriminator.Value);
            }

            serializerSettings.Converters.Add(jsonSubtypesConverterBuilder.Build());
            var jsonSettings = JsonConvert.DeserializeObject<AppHealthCheckDataSource>(json, serializerSettings);
            var settings = new List<HealthCheckItem>();
            settings.AddRange(jsonSettings.HealthChecks);
            settings.AddRange(jsonSettings.Groups.SelectMany(x =>
            {
                x.Value.ForEach(p => p.Group = x.Key);
                return x.Value;
            }).ToList());
            return settings;
        }
    }
}