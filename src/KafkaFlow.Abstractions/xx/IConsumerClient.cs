


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaFlow.Consumers
{
    /// <inheritdoc />
    /// <summary>
    /// Message queue consumer client
    /// </summary>
    public interface IConsumerClient : IDisposable
    {
        //string BrokerAddress { get; }

        /// <summary>
        /// Subscribe to a set of topics to the message queue
        /// </summary>
        /// <param name="topics"></param>
        //void Subscribe(IEnumerable<string> topics);

        /// <summary>
        /// Start listening
        /// </summary>
        //void Listening(TimeSpan timeout, CancellationToken cancellationToken);

        /// <summary>
        /// Manual submit message offset when the message consumption is complete
        /// </summary>
        void Commit(IEnumerable<XXXTopicPartitionOffset> offsets);
        void Pause(IEnumerable<XXXTopicPartition> offsets);
        void Resume(IEnumerable<XXXTopicPartition> offsets);
        List<XXXTopicPartition> Assignment { get; }
        string Name { get; }
        string MemberId { get; }
        IReadOnlyList<string> Subscription { get; }
        long Position(XXXTopicPartition offsets);
        IOffsetsWatermark GetWatermarkOffsets(XXXTopicPartition offsets);
        IOffsetsWatermark QueryWatermarkOffsets(XXXTopicPartition offsets, TimeSpan timeout);
        List<XXXTopicPartitionOffset> OffsetsForTimes(IEnumerable<XXXTopicPartitionTimestamp> timestampsToSearch, TimeSpan timeout);
        Task StartAsync();

        Task StopAsync();
        /// <summary>
        /// Reject message and resumption
        /// </summary>
        //void Reject(object sender);
        void OnPartitionRevoked(IConsumerClient consumer, IReadOnlyCollection<XXXTopicPartitionOffset> topicPartitions);
        void OnPartitionAssigned(IConsumerClient consumer, IReadOnlyCollection<XXXTopicPartition> partitions);
    }
}
