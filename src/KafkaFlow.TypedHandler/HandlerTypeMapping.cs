namespace KafkaFlow.TypedHandler
{
    using System;
    using System.Linq;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using Volte.Data.VolteDi;

    [Injection(InjectionType = InjectionType.Auto)]
    public class HandlerTypeMapping: IHandlerTypeMapping
    {
        private readonly ConcurrentDictionary<Type, ICollection<IMessageHandler>> _messageHandlers = new ConcurrentDictionary<Type, ICollection<IMessageHandler>>();
        IVolteDiServiceProvider VolteDiServiceProvider;

        public HandlerTypeMapping(
            IVolteDiServiceProvider VolteDiServiceProvider)
        {
            this.VolteDiServiceProvider= VolteDiServiceProvider;
        }
        public ICollection<IMessageHandler> GetMessageHandler(Type eventType)
        {
            var eventHandlerType = typeof(IMessageHandler<>).MakeGenericType(eventType);
            Console.WriteLine("eventType--->" + eventType.ToString());
            Console.WriteLine("HandlerTypeMapping--->" + eventHandlerType.ToString());
            var xx1 = VolteDiServiceProvider.ServiceProvider.GetGenericTypeServices(eventHandlerType);

            var xx = VolteDiServiceProvider.ServiceProvider.GetGenericTypeServices(eventHandlerType).Cast<IMessageHandler>().ToArray();
            return xx;
        }
        public ICollection<IMessageHandler> GetHandlers(Type eventType)
        {
            var eventHandlers = _messageHandlers.GetOrAdd(eventType, type =>
            {
                return this.GetMessageHandler(eventType);
            });
            
           var x2=this.GetMessageHandler(eventType);

            return eventHandlers;
        }

    }
}