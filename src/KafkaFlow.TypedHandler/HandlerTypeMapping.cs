namespace KafkaFlow.TypedHandler
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    public class HandlerTypeMapping
    {
        private readonly ConcurrentDictionary<Type, ICollection<IMessageHandler>> _messageHandlers = new ConcurrentDictionary<Type, ICollection<IMessageHandler>>();
        private readonly IDependencyResolver dependencyResolver;
        public HandlerTypeMapping(
            IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }
        public ICollection<IMessageHandler> GetHandlers(Type eventType)
        {
            var eventHandlers = _messageHandlers.GetOrAdd(eventType, type =>
            {
                ICollection<IMessageHandler> handlers = new List<IMessageHandler>();
                foreach (var handlerType in dependencyResolver.Resolves<IMessageHandler>())
                {
                    Console.WriteLine("GetHandlers." + handlerType);

                    foreach (var implementedInterface in handlerType.GetType().GetTypeInfo().ImplementedInterfaces)
                    {
                        if (implementedInterface.IsGenericType && eventType.IsAssignableFrom(implementedInterface.GenericTypeArguments[0]))
                        {
                            handlers.Add(handlerType);
                        }
                    }
                }
                return handlers;
            });
            return eventHandlers;
        }
    }
}