namespace MessagePipeline.IntegrationTests.Core.Handlers
{
    using System.Threading.Tasks;
    using MessagePipeline.TypedHandler;
    using Messages;

    public class MessageHandler : IMessageHandler<TestMessage1>
    {
        public Task Handle(IMessageContext context, TestMessage1 message)
        {
            MessageStorage.Add(message);
            return Task.CompletedTask;
        }
    }
}
