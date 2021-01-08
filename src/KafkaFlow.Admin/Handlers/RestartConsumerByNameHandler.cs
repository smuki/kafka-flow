namespace MessagePipeline.Admin.Handlers
{
    using System.Threading.Tasks;
    using MessagePipeline.Admin.Messages;
    using MessagePipeline.Consumers;
    using MessagePipeline.TypedHandler;

    internal class RestartConsumerByNameHandler : IMessageHandler<RestartConsumerByName>
    {
        private readonly IConsumerAccessor consumerAccessor;

        public RestartConsumerByNameHandler(IConsumerAccessor consumerAccessor) => this.consumerAccessor = consumerAccessor;

        public Task Handle(IMessageContext context, RestartConsumerByName message)
        {
            var consumer = this.consumerAccessor[message.ConsumerName];

            return consumer?.RestartAsync() ?? Task.CompletedTask;
        }
    }
}
