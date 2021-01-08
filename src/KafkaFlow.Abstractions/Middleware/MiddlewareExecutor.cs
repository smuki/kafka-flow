namespace MessagePipeline.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Volte.Data.VolteDi;
    using Volte.Utils;

    [Injection(InjectionType = InjectionType.Auto)]
    public class MiddlewareExecutor : IMiddlewareExecutor
    {
        private IReadOnlyList<IMessageMiddleware> middlewares;
        public void Initialize(IReadOnlyList<IMessageMiddleware> middlewares)
        {
            this.middlewares = middlewares;
        }

        public Task Execute(IMessageContext context, Func<IMessageContext, Task> nextOperation)
        {
            int i = 0;
            NLogger.Debug("Middleware: ");
            foreach (var x in middlewares)
            {
                NLogger.Debug("   " + i + " : " + x.ToString());
                i++;
            }
            return this.ExecuteDefinition(0, context, nextOperation);
        }

        private Task ExecuteDefinition(
            int index,
            IMessageContext context,
            Func<IMessageContext, Task> nextOperation)
        {
            if (this.middlewares.Count == index)
            {
                return nextOperation(context);
            }
            return this.middlewares[index].Invoke(context, nextContext => this.ExecuteDefinition(index + 1, nextContext.Clone(), nextOperation));
        }
    }
}
