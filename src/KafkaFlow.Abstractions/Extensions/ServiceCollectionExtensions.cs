using System;
using System.Linq;
using MessagePipeline.Consumers;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseKafkaFlow(this IServiceCollection services)
        {
            var consumerManager = new ConsumerManager();

            services.AddSingleton<IConsumerAccessor>(consumerManager);
            services.AddSingleton<IConsumerAccessor>(consumerManager);
            services.AddSingleton<IConsumerManager>(consumerManager);
                    
            return services;
        }

        public static IServiceCollection Replace<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime) => services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        public static bool HasService<T>(this IServiceCollection services) => services.Any(x => x.ServiceType == typeof(T));
    }
}