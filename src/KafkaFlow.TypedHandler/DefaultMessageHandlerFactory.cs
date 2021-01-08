namespace MessagePipeline.TypedHandler
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Volte.Data.VolteDi;

    [Injection(InjectionType = InjectionType.Auto)]
    public class DefaultMessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly ConcurrentDictionary<Type, ICollection<IMessageHandler>> _messageHandlers = new ConcurrentDictionary<Type, ICollection<IMessageHandler>>();
        private readonly IVolteServiceResolver VolteDiServiceProvider;

        public DefaultMessageHandlerFactory(
            IVolteServiceResolver VolteDiServiceProvider)
        {
            this.VolteDiServiceProvider = VolteDiServiceProvider;
        }
        public ICollection<IMessageHandler> GetMessageHandler(Type eventType)
        {
            var eventHandlerType = typeof(IMessageHandler<>).MakeGenericType(eventType);
            return VolteDiServiceProvider.GetGenericTypeServices(eventHandlerType).Cast<IMessageHandler>().ToArray();
        }
        public ICollection<IMessageHandler> GetMessageHandlers(Type eventType)
        {
            var eventHandlers = _messageHandlers.GetOrAdd(eventType, type =>
            {
                return this.GetMessageHandler(eventType);
            });
            return eventHandlers;
        }
    }
}