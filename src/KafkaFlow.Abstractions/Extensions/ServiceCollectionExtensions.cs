﻿using System;
using System.Linq;
using System.Net.Security;
using KafkaFlow;
using KafkaFlow.Consumers;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the specified service only if none already exists for the specified provider type.
        /// </summary>
        public static IServiceCollection TryAddProvider<TService, TProvider>(
            this IServiceCollection services,
            ServiceLifetime lifetime)
        {
            return services.TryAddProvider(typeof(TService), typeof(TProvider), lifetime);
        }

        /// <summary>
        /// Registers the specified service only if none already exists for the specified provider type.
        /// </summary>
        public static IServiceCollection TryAddProvider(
            this IServiceCollection services,
            Type serviceType,
            Type providerType,
            ServiceLifetime lifetime)
        {
            var descriptor = services.FirstOrDefault(
                x => x.ServiceType == serviceType && x.ImplementationType == providerType
            );

            if (descriptor == null)
            {
                descriptor = new ServiceDescriptor(serviceType, providerType, lifetime);
                services.Add(descriptor);
            }

            return services;
        }
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
        public static bool HasService<T>(this KafkaFlowOptions options) => options.Services.HasService<T>();
    }
}