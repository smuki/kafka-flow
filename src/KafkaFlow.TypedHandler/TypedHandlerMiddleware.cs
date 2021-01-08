namespace MessagePipeline.TypedHandler
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Volte.Data.VolteDi;

    [Injection(InjectionType = InjectionType.Auto)]
    [Middleware(MiddlewareType = MiddlewareType.Consumer,Priority =110)]
    public class TypedHandlerMiddleware : IMessageMiddleware
    {
        private readonly IMessageHandlerFactory HandlerMapping;

        public TypedHandlerMiddleware(
            IMessageHandlerFactory HandlerMapping)
        {
            this.HandlerMapping = HandlerMapping;
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var handlerType = HandlerMapping.GetMessageHandlers(context.Message.GetType());

            if (handlerType == null || handlerType.Count == 0)
            {
                return;
            }
            var tasks = handlerType.Select(h =>
           {
               return HandlerExecutor.GetExecutor(context.Message.GetType()).Execute(h, context, context.Message);
           });
            await Task.WhenAll(tasks);

            await next(context);
        }
    }
}
