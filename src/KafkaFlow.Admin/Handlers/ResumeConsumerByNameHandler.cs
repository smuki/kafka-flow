namespace MessagePipeline.Admin.Handlers
{
    using System.Threading.Tasks;
    using MessagePipeline.Admin.Messages;
    using MessagePipeline.Consumers;
    using MessagePipeline.TypedHandler;

    internal class ResumeConsumerByNameHandler : IMessageHandler<ResumeConsumerByName>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public ResumeConsumerByNameHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, ResumeConsumerByName message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            consumer?.Resume(consumer.Assignment);

            return Task.CompletedTask;
        }
    }
}
