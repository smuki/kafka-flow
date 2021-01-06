namespace KafkaFlow.TypedHandler
{
    using System;
    using System.Linq;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using Volte.Data.VolteDi;

    public interface IHandlerTypeMapping
    {
        public ICollection<IMessageHandler> GetHandlers(Type eventType);
    }
}