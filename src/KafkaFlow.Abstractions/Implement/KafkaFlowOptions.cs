using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow
{
    public class KafkaFlowOptions
    {
        public IServiceCollection Services { get; }

        public KafkaFlowOptions(IServiceCollection services)
        {
            Services = services;
        }
    }
}
