namespace MessagePipeline.TypedHandler
{
    using System;
    using System.Linq;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using Volte.Data.VolteDi;

    public interface IMessageHandlerFactory
    {
        public ICollection<IMessageHandler> GetMessageHandlers(Type eventType);
    }
}