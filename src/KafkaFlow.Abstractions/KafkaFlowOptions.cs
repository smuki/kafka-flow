using KafkaFlow.Consumers;
using KafkaFlow.Middleware;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KafkaFlow
{
    public class KafkaFlowOptions
    {
        public KafkaFlowOptions(IServiceCollection services)
        {
            this.Services = services;

            this.Services.TryAddProvider<IMiddlewareExecutor, MiddlewareExecutor>(ServiceLifetime.Transient);
            this.Services.TryAddProvider<IConsumerWorkerPool, ConsumerWorkerPool>(ServiceLifetime.Singleton);
            this.Services.TryAddProvider<IConsumerAccessor, ConsumerManager>(ServiceLifetime.Singleton);
            this.Services.TryAddProvider<IConsumerManager, ConsumerManager>(ServiceLifetime.Singleton);

        }
        public KafkaFlowOptions AddWorkflowsFrom(Assembly assembly)
        {

            //var types = assembly.GetAllWithInterface<>();

            //foreach (var type in types)
            //{
            //    AddWorkflow(type);
            //}
            return this;
        }
        public IServiceCollection Services { get; }

    }
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetAllWithInterface(this Assembly assembly, Type @interface) => assembly.GetTypes().Where(t => t.IsClass && t.IsAbstract == false && t.GetInterfaces().Contains(@interface));
        public static IEnumerable<Type> GetAllWithInterface<TType>(this Assembly assembly) => assembly.GetAllWithInterface(typeof(TType));
    }
}
