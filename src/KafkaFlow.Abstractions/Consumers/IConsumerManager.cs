namespace MessagePipeline.Consumers
{
    public interface IConsumerManager : IConsumerAccessor
    {
        void AddOrUpdate(IMessageConsumer consumer);
    }
}
