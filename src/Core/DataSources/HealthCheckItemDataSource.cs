using System.Collections.Generic;
using HealthCheck.AspNetCore.Plus.Models;

namespace HealthCheck.AspNetCore.Plus.DataSources
{
    public class HealthCheckItemDataSource<T> : IAppHealthCheckDataSource where T : HealthCheckItem
    {
        private readonly T _item;

        public HealthCheckItemDataSource(T item)
        {
            _item = item;
        }
        
        public List<HealthCheckItem> Retrieve()
        {
            return new List<HealthCheckItem>() { _item };
        }
    }
}