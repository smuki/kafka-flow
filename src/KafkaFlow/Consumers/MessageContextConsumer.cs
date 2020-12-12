namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;
    using Confluent.Kafka;

    internal class MessageContextConsumer : IMessageContextConsumer
    {
        private readonly IConsumer<byte[], byte[]> consumer;
        private readonly IOffsetManager offsetManager;
        private readonly IntermediateMessage kafkaResult;

        public MessageContextConsumer(
            IConsumer<byte[], byte[]> consumer,
            string name,
            IOffsetManager offsetManager,
            IntermediateMessage kafkaResult,
            CancellationToken workerStopped)
        {
            this.Name = name;
            this.WorkerStopped = workerStopped;
            this.consumer = consumer;
            this.offsetManager = offsetManager;
            this.kafkaResult = kafkaResult;
        }

        public string Name { get; }

        public CancellationToken WorkerStopped { get; }

        public bool ShouldStoreOffset { get; set; } = true;

        public DateTime MessageTimestamp => this.kafkaResult.Timestamp.UtcDateTime;

        public void StoreOffset()
        {
            this.offsetManager.StoreOffset(this.kafkaResult.TopicPartitionOffset);
        }

        public IOffsetsWatermark GetOffsetsWatermark()
        {
            WatermarkOffsets v = this.consumer.GetWatermarkOffsets(Util.TopicPartition(this.kafkaResult.TopicPartition));

            return new OffsetsWatermark(v.High,v.Low);
        }

        public void Pause()
        {
            this.consumer.Pause(this.consumer.Assignment);
        }

        public void Resume()
        {
            this.consumer.Resume(this.consumer.Assignment);
        }
    }
}
