namespace KafkaFlow.TypedHandler
{
    using System.Linq;
    using System.Threading.Tasks;
    using Volte.Data.VolteDi;

    [Injection(InjectionType = InjectionType.Auto)]
    [Middleware(MiddlewareType = MiddlewareType.Consumer)]
    public class TypedHandlerMiddleware : IMessageMiddleware
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly HandlerTypeMapping HandlerMapping;

        public TypedHandlerMiddleware(
            IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
            this.HandlerMapping = new HandlerTypeMapping(dependencyResolver);
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            using (var scope = this.dependencyResolver.CreateScope())
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
            }
            await next(context);
        }
    }
}
