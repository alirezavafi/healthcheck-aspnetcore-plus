using System.Collections.Generic;
using HealthCheck.AspNetCore.Plus.Models;

namespace HealthCheck.AspNetCore.Plus.DataSources
{
    public interface IAppHealthCheckDataSource
    {
        List<HealthCheckItem> Retrieve();
    }
}