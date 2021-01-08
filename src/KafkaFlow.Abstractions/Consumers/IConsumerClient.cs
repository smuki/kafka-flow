using MessagePipeline.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MessagePipeline.Consumers
{
    /// <summary>
    /// Message queue consumer client
    /// </summary>
    public interface IConsumerClient : IDisposable
    {
        void Initialize(IConsumerWorkerPool consumerWorkerPool, MessageConsumerSettting eventConsumer,CancellationToken busStopCancellationToken);

        void Commit(IEnumerable<XXXTopicPartitionOffset> offsets);

        void Pause(IEnumerable<XXXTopicPartition> offsets);

        void Resume(IEnumerable<XXXTopicPartition> offsets);

        List<XXXTopicPartition> Assignment { get; }
        string this[string name] { get; }
        
        string Name { get; }
        string MemberId { get; }
        string ConsumerName { get; }
        MessageConsumerSettting Parameter { get; }
        IReadOnlyList<string> Subscription { get; }
        long Position(XXXTopicPartition offsets);
        IOffsetsWatermark GetWatermarkOffsets(XXXTopicPartition offsets);
        IOffsetsWatermark QueryWatermarkOffsets(XXXTopicPartition offsets, TimeSpan timeout);
        List<XXXTopicPartitionOffset> OffsetsForTimes(IEnumerable<XXXTopicPartitionTimestamp> timestampsToSearch, TimeSpan timeout);
        Task StartAsync();

        Task StopAsync();
    }
}
