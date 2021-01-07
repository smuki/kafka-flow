using System;
using System.Linq;
using System.Net.Security;
using KafkaFlow;
using KafkaFlow.Consumers;
using KafkaFlow.Producers;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseKafkaFlow(this IServiceCollection services)
        {
            var consumerManager = new ConsumerManager();

            services.AddTransient(typeof(ILogHandler), typeof(NullLogHandler));
            services.AddTransient<ILogHandler, NullLogHandler>();
            services.AddSingleton<IConsumerAccessor>(consumerManager);
            services.AddSingleton<IConsumerAccessor>(consumerManager);
            services.AddSingleton<IConsumerManager>(consumerManager);
                    
            return services;
        }

        public static IServiceCollection Replace<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime) => services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        public static bool HasService<T>(this IServiceCollection services) => services.Any(x => x.ServiceType == typeof(T));
    }
}