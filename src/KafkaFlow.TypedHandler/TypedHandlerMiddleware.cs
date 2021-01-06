namespace KafkaFlow.TypedHandler
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Volte.Data.VolteDi;

    [Injection(InjectionType = InjectionType.Auto)]
    [Middleware(MiddlewareType = MiddlewareType.Consumer,Priority =110)]
    public class TypedHandlerMiddleware : IMessageMiddleware
    {
        private readonly IHandlerTypeMapping HandlerMapping;

        public TypedHandlerMiddleware(
            IHandlerTypeMapping HandlerMapping)
        {
            this.HandlerMapping = HandlerMapping;
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var handlerType = HandlerMapping.GetHandlers(context.Message.GetType());

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
