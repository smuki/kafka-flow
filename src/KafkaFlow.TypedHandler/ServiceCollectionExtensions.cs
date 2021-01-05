//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using KafkaFlow;
//using KafkaFlow.TypedHandler;
//using Microsoft.Extensions.DependencyInjection.Extensions;

//// ReSharper disable once CheckNamespace
//namespace Microsoft.Extensions.DependencyInjection
//{
//    public static class ServiceCollectionExtensions
//    {
//        public static IServiceCollection ScanMessageHandler(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient, params Assembly[] assemblies)
//        {
//            if (assemblies == null || assemblies.Length == 0)
//            {
//                //assemblies = Helpers.ReflectHelper.GetAssemblies();
//            }

//            var handlerTypes = assemblies
//                .Select(ass => ass.GetTypes())
//                .SelectMany(t => t)
//                .Where(t => !t.IsAbstract && typeof(IMessageHandler).IsAssignableFrom(t));
//            //if (filter != null)
//            //{
//            //    handlerTypes = handlerTypes.Where(filter);
//            //}

//            foreach (var handlerType in handlerTypes)
//            {
//                foreach (var implementedInterface in handlerType.GetTypeInfo().ImplementedInterfaces)
//                {
//                    //if (implementedInterface.IsGenericType && typeof(IEventBase).IsAssignableFrom(implementedInterface.GenericTypeArguments[0]))
//                    if (implementedInterface.IsGenericType)
//                    {
//                        Console.Write(implementedInterface.Name);
//                        Console.Write("   "+implementedInterface.GenericTypeArguments[0].Name);
//                        Console.WriteLine(" ----> " + handlerType.Name);

//                        services.TryAddEnumerable(new ServiceDescriptor(implementedInterface, handlerType, serviceLifetime));
//                    }
//                }
//            }

//            return services;
//        }

//        /// <summary>
//        /// 获取命名服务
//        /// </summary>
//        /// <param name="name">服务名称</param>
//        public static ICollection<IMessageHandler> GetHandlers(this IServiceProvider provider, Type eventType)
//        {
//            var eventHandlerType = typeof(IMessageHandler<>).MakeGenericType(eventType);
//            return provider.GetServices(eventHandlerType).Cast<IMessageHandler>().ToArray();
//        }
//        /// <summary>
//        /// Registers the specified service only if none already exists for the specified provider type.
//        /// </summary>
//        //public static IServiceCollection TryAddProvider<TService, TProvider>(
//        //    this IServiceCollection services,
//        //    ServiceLifetime lifetime)
//        //{
//        //    return services.TryAddProvider(typeof(TService), typeof(TProvider), lifetime);
//        //}

//        /// <summary>
//        /// Registers the specified service only if none already exists for the specified provider type.
//        /// </summary>
//        //public static IServiceCollection TryAddProvider(
//        //    this IServiceCollection services,
//        //    Type serviceType,
//        //    Type providerType,
//        //    ServiceLifetime lifetime)
//        //{
//        //    var descriptor = services.FirstOrDefault(
//        //        x => x.ServiceType == serviceType && x.ImplementationType == providerType
//        //    );

//        //    if (descriptor == null)
//        //    {
//        //        descriptor = new ServiceDescriptor(serviceType, providerType, lifetime);
//        //        services.Add(descriptor);
//        //    }

//        //    return services;
//        //}

//        //public static IServiceCollection Replace<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime) => services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
//        //public static bool HasService<T>(this IServiceCollection services) => services.Any(x => x.ServiceType == typeof(T));
//        //public static bool HasService<T>(this KafkaFlowOptions options) => options.Services.HasService<T>();
//    }
//}