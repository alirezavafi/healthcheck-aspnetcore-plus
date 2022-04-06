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
        private readonly AppHealthCheckOptions _appHealthCheckOptions;
        public FileAppHealthCheckDataSource(AppHealthCheckOptions appHealthCheckOptions)
        {
            _appHealthCheckOptions = appHealthCheckOptions;
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

        internal string FileName { get; private set; }
        internal bool IsOptional { get; private set; }

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
                .SerializeDiscriminatorProperty();
            foreach (var typeDiscriminator in _appHealthCheckOptions.FileDataSourceDiscriminators)
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