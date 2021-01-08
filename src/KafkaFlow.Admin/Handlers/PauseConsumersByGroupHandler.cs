namespace MessagePipeline.Admin.Handlers
{
    using System.Linq;
    using System.Threading.Tasks;
    using MessagePipeline.Admin.Messages;
    using MessagePipeline.Consumers;
    using MessagePipeline.TypedHandler;

    internal class PauseConsumersByGroupHandler : IMessageHandler<PauseConsumersByGroup>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public PauseConsumersByGroupHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, PauseConsumersByGroup message)
        {
            var consumers = this.consumerAccessor.Consumers.Where(x => x.GroupId == message.GroupId);

            foreach (var consumer in consumers)
            {
                consumer.Pause(consumer.Assignment);
            }

            return Task.CompletedTask;
        }
    }
}
