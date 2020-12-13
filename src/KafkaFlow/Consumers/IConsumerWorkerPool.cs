namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;

    internal interface IConsumerWorkerPool
    {
        Task StartAsync(
            IConsumerClient consumerClient,
            IConsumer<byte[], byte[]> consumer,
            IEnumerable<XXXTopicPartition> partitions,
            CancellationToken stopCancellationToken);

        Task StopAsync();

        Task EnqueueAsync(
            IntermediateMessage message,
            CancellationToken stopCancellationToken);
    }
}
