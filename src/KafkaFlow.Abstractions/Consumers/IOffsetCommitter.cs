namespace KafkaFlow.Consumers
{
    using System;

    public interface IOffsetCommitter : IDisposable
    {
        void StoreOffset(XXXTopicPartitionOffset tpo);
    }
}
