namespace MessagePipeline.Admin.Handlers
{
    using System.Threading.Tasks;
    using MessagePipeline.Admin.Messages;
    using MessagePipeline.Consumers;
    using MessagePipeline.TypedHandler;

    internal class ChangeConsumerWorkersCountHandler : IMessageHandler<ChangeConsumerWorkerCount>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public ChangeConsumerWorkersCountHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, ChangeConsumerWorkerCount message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            return
                consumer?.ChangeWorkerCountAndRestartAsync(message.WorkerCount) ??
                Task.CompletedTask;
        }
    }
}
