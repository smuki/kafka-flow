namespace MessagePipeline.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface IMiddlewareExecutor
    {
        void Initialize(IReadOnlyList<IMessageMiddleware> middlewares);

        Task Execute(IMessageContext context, Func<IMessageContext, Task> nextOperation);
    }
}
