using System;

namespace HealthCheck.AspNetCore.Plus
{
    public class FileDataSourceDiscriminatorAttribute : Attribute
    {
        public string Discriminator { get; }

        public FileDataSourceDiscriminatorAttribute(string discriminator)
        {
            Discriminator = discriminator;
        }
    }
}