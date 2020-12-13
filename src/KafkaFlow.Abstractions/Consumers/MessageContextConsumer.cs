namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading;

    public class MessageContextConsumer : IMessageContextConsumer
    {
        private readonly IOffsetManager offsetManager;
        private readonly IntermediateMessage kafkaResult;
        private readonly IConsumerClient consumerClient;

        public MessageContextConsumer(
            IConsumerClient consumerClient,
            string name,
            IOffsetManager offsetManager,
            IntermediateMessage kafkaResult,
            CancellationToken workerStopped)
        {
            this.Name = name;
            this.WorkerStopped = workerStopped;
            this.consumerClient = consumerClient;
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
            return this.consumerClient.GetWatermarkOffsets(this.kafkaResult.TopicPartition);
        }

        public void Pause()
        {
            this.consumerClient.Pause(this.consumerClient.Assignment);
        }

        public void Resume()
        {
            this.consumerClient.Resume(this.consumerClient.Assignment);
        }
    }
}
