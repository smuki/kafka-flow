namespace MessagePipeline.Consumers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MessagePipeline.Configuration;
    public interface IConsumerWorkerPool
    {
        void Initialize(MessageConsumerSettting eventConsumer);

        Task StartAsync(IConsumerClient consumerClient, IEnumerable<XXXTopicPartition> partitions, CancellationToken stopCancellationToken);

        Task StopAsync();

        Task EnqueueAsync(IntermediateMessage message, CancellationToken stopCancellationToken);
    }
}
