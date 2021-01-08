namespace MessagePipeline.Consumers
{
    public interface IOffsetManager
    {
        void StoreOffset(XXXTopicPartitionOffset offset);
    }
}
