namespace KafkaFlow.TypedHandler
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
        private readonly IVolteDiServiceProvider VolteDiServiceProvider;

        public DefaultMessageHandlerFactory(
            IVolteDiServiceProvider VolteDiServiceProvider)
        {
            this.VolteDiServiceProvider = VolteDiServiceProvider;
        }
        public ICollection<IMessageHandler> GetMessageHandler(Type eventType)
        {
            var eventHandlerType = typeof(IMessageHandler<>).MakeGenericType(eventType);
            Console.WriteLine("eventType--->" + eventType.ToString());
            Console.WriteLine("HandlerTypeMapping--->" + eventHandlerType.ToString());
            return VolteDiServiceProvider.ServiceProvider.GetGenericTypeServices(eventHandlerType).Cast<IMessageHandler>().ToArray();
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