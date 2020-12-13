namespace KafkaFlow.Middleware
{
    using System;
    using System.Threading.Tasks;

    public interface IMiddlewareExecutor
    {
        Task Execute(IMessageContext context, Func<IMessageContext, Task> nextOperation);
    }
}
