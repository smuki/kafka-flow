namespace KafkaFlow.TypedHandler
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class HandlerTypeMapping
    {
        private readonly Dictionary<Type, Type> mapping = new Dictionary<Type, Type>();
        private readonly ConcurrentDictionary<Type, ICollection<IMessageHandler>> _eventHandlers = new ConcurrentDictionary<Type, ICollection<IMessageHandler>>();

        private readonly IDependencyResolver dependencyResolver;

        public HandlerTypeMapping(
            IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void AddMapping(Type messageType, Type handlerType)
        {
            this.mapping.Add(messageType, handlerType);
        }

        public Type GetHandlerType(Type messageType)
        {
            return this.mapping.TryGetValue(messageType, out var handlerType) ? handlerType : null;
        }

        public ICollection<IMessageHandler> GetHandlers(Type eventType)
        {
            var eventHandlers = _eventHandlers.GetOrAdd(eventType, type =>
            {
                ICollection<IMessageHandler> handlers = new List<IMessageHandler>();
                foreach (var handlerType in dependencyResolver.Resolves<IMessageHandler>())
                {
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