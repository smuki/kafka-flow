namespace KafkaFlow.Consumers
{
    public interface IOffsetManager
    {
        void StoreOffset(XXXTopicPartitionOffset offset);
    }
}
