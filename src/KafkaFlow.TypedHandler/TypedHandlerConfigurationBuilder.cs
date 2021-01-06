namespace KafkaFlow.TypedHandler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Volte.Data.VolteDi;

    /// <summary>
    /// Builder class for typed handler configuration
    /// </summary>
    public class TypedHandlerConfigurationBuilder
    {
        private readonly IDependencyConfigurator dependencyConfigurator;

        /// <summary>
        /// </summary>
        /// <param name="dependencyConfigurator">Dependency injection configurator</param>
        public TypedHandlerConfigurationBuilder(IDependencyConfigurator dependencyConfigurator)
        {
            this.dependencyConfigurator = dependencyConfigurator;
        }

        /// <summary>
        /// Adds all classes that implements the <see cref="IMessageHandler{TMessage}"/> interface from the assembly of the provided type
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="IMessageHandler{TMessage}"/> interface</typeparam>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder AddHandlersFromAssemblyOf<T>()
            where T : IMessageHandler
        {
            return this;
        }

        /// <summary>
        /// Manually adds the message handler
        /// </summary>
        /// <typeparam name="T">A type that implements the <see cref="IMessageHandler{TMessage}"/> interface</typeparam>
        /// <returns></returns>
        public TypedHandlerConfigurationBuilder AddHandler<T>()
            where T : class, IMessageHandler
        {
            return this;
        }
        //public static TypedHandlerConfigurationBuilder RegisterEventHandlers(this TypedHandlerConfigurationBuilder builder, params Assembly[] assemblies)
        //{
        //    if (assemblies == null || assemblies.Length == 0)
        //    {
        //        return builder;
        //    }

        //    var handlerTypes = assemblies
        //        .Select(ass => ass.GetTypes())
        //        .SelectMany(t => t)
        //        .Where(t => !t.IsAbstract && typeof(IMessageHandler).IsAssignableFrom(t));

        //    foreach (var handlerType in handlerTypes)
        //    {
        //        foreach (var implementedInterface in handlerType.GetTypeInfo().ImplementedInterfaces)
        //        {
        //            if (implementedInterface.IsGenericType)
        //            {
        //                dependencyConfigurator.TryAddEnumerable(new ServiceDescriptor(implementedInterface, handlerType, serviceLifetime));
        //            }
        //        }
        //    }

        //    return builder;
        //}
    }
}
