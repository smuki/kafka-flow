namespace MessagePipeline.Admin.Handlers
{
    using System.Linq;
    using System.Threading.Tasks;
    using MessagePipeline.Admin.Messages;
    using MessagePipeline.Consumers;
    using MessagePipeline.TypedHandler;

    internal class ResumeConsumersByGroupHandler : IMessageHandler<ResumeConsumersByGroup>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public ResumeConsumersByGroupHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, ResumeConsumersByGroup message)
        {
            var consumers = this.consumerAccessor.Consumers.Where(x => x.GroupId == message.GroupId);

            foreach (var consumer in consumers)
            {
                consumer.Resume(consumer.Assignment);
            }

            return Task.CompletedTask;
        }
    }
}
