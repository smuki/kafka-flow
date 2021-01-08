namespace MessagePipeline.Admin.Handlers
{
    using System.Linq;
    using System.Threading.Tasks;
    using MessagePipeline.Admin.Messages;
    using MessagePipeline.Consumers;
    using MessagePipeline.TypedHandler;

    internal class PauseConsumersByGroupTopicHandler : IMessageHandler<PauseConsumersByGroupTopic>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public PauseConsumersByGroupTopicHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, PauseConsumersByGroupTopic message)
        {
            var consumers = this.consumerAccessor.Consumers
                .Where(x => x.GroupId == message.GroupId);

            foreach (var consumer in consumers)
            {
                consumer.Pause(consumer.Assignment.Where(x => x.Topic == message.Topic));
            }

            return Task.CompletedTask;
        }
    }
}
