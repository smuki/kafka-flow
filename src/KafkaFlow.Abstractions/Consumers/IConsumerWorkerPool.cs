namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IConsumerWorkerPool
    {
        Task StartAsync(IConsumerClient consumerClient, IEnumerable<XXXTopicPartition> partitions, CancellationToken stopCancellationToken);

        Task StopAsync();

        Task EnqueueAsync(IntermediateMessage message, CancellationToken stopCancellationToken);
    }
}
