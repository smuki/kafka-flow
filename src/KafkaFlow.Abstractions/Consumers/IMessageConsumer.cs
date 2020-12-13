namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides access to the kafka message consumer
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// Gets the unique consumer´s name defined in the configuration
        /// </summary>
        string ConsumerName { get; }

        /// <summary>
        /// Gets the group id define in the configuration
        /// </summary>
        string GroupId { get; }

        /// <summary>
        /// Gets the current topic subscription
        /// </summary>
        IReadOnlyList<string> Subscription { get; }

        /// <summary>
        /// Gets the current partition assignment
        /// </summary>
        IReadOnlyList<XXXTopicPartition> Assignment { get; }

        /// <summary>
        /// Gets the (dynamic) group member id of this consumer (as set by the broker).
        /// </summary>
        string MemberId { get; }

        /// <summary>
        ///     Gets the name of this client instance.
        /// 
        ///     Contains (but is not equal to) the client.id
        ///     configuration parameter.
        /// </summary>
        /// <remarks>
        ///     This name will be unique across all client
        ///     instances in a given application which allows
        ///     log messages to be associated with the
        ///     corresponding instance.
        /// </remarks>
        string ClientInstanceName { get; }

        /// <summary>
        /// Overrides the offsets of the given partitions and restart the consumer
        /// </summary>
        /// <param name="offsets">The offset values</param>
        Task OverrideOffsetsAndRestartAsync(IReadOnlyCollection<XXXTopicPartitionOffset> offsets);

        /// <summary>
        /// Restart the current consumer with the new worker count
        /// </summary>
        /// <param name="workerCount">The new worker count</param>
        /// <returns></returns>
        Task ChangeWorkerCountAndRestartAsync(int workerCount);

        /// <summary>
        /// Restart KafkaFlow consumer and recreate the internal Confluent Consumer 
        /// </summary>
        /// <returns></returns>
        Task RestartAsync();

        /// <summary>
        ///     Pause consumption for the provided list
        ///     of partitions.
        /// </summary>
        /// <param name="partitions">
        ///     The partitions to pause consumption of.
        /// </param>
        /// <exception cref="T:Confluent.Kafka.KafkaException">
        ///     Thrown if the request failed.
        /// </exception>
        /// <exception cref="T:Confluent.Kafka.TopicPartitionException">
        ///     Per partition success or error.
        /// </exception>
        void Pause(IEnumerable<XXXTopicPartition> partitions);

        /// <summary>
        ///     Resume consumption for the provided list of partitions.
        /// </summary>
        /// <param name="partitions">
        ///     The partitions to resume consumption of.
        /// </param>
        /// <exception cref="T:Confluent.Kafka.KafkaException">
        ///     Thrown if the request failed.
        /// </exception>
        /// <exception cref="T:Confluent.Kafka.TopicPartitionException">
        ///     Per partition success or error.
        /// </exception>
        void Resume(IEnumerable<XXXTopicPartition> partitions);

        /// <summary>
        ///     Gets the current position (offset) for the
        ///     specified topic / partition.
        /// 
        ///     The offset field of each requested partition
        ///     will be set to the offset of the last consumed
        ///     message + 1, or Offset.Unset in case there was
        ///     no previous message consumed by this consumer.
        /// </summary>
        /// <exception cref="T:Confluent.Kafka.KafkaException">
        ///     Thrown if the request failed.
        /// </exception>
        long GetPosition(XXXTopicPartition topicPartition);

        /// <summary>
        ///     Get the last cached low (oldest available /
        ///     beginning) and high (newest/end) offsets for
        ///     a topic/partition. Does not block.
        /// </summary>
        /// <remarks>
        ///     The low offset is updated periodically (if
        ///     statistics.interval.ms is set) while the
        ///     high offset is updated on each fetched
        ///     message set from the broker. If there is no
        ///     cached offset (either low or high, or both)
        ///     then Offset.Unset will be returned for the
        ///     respective offset.
        /// </remarks>
        /// <param name="topicPartition">
        ///     The topic partition of interest.
        /// </param>
        /// <returns>
        ///     The requested WatermarkOffsets
        ///     (see that class for additional documentation).
        /// </returns>
        IOffsetsWatermark GetWatermarkOffsets(XXXTopicPartition topicPartition);

        /// <summary>
        ///     Query the Kafka cluster for low (oldest
        ///     available/beginning) and high (newest/end)
        ///     offsets for the specified topic/partition.
        ///     This is a blocking call - always contacts
        ///     the cluster for the required information.
        /// </summary>
        /// <param name="topicPartition">
        ///     The topic/partition of interest.
        /// </param>
        /// <param name="timeout">
        ///     The maximum period of time
        ///     the call may block.
        /// </param>
        /// <returns>
        ///     The requested WatermarkOffsets (see
        ///     that class for additional documentation).
        /// </returns>
        IOffsetsWatermark QueryWatermarkOffsets(XXXTopicPartition topicPartition, TimeSpan timeout);

        /// <summary>
        ///     Look up the offsets for the given partitions
        ///     by timestamp. The returned offset for each
        ///     partition is the earliest offset whose
        ///     timestamp is greater than or equal to the
        ///     given timestamp in the corresponding partition.
        /// </summary>
        /// <remarks>
        ///     The consumer does not need to be assigned to
        ///     the requested partitions.
        /// </remarks>
        /// <param name="timestampsToSearch">
        ///     The mapping from partition
        ///     to the timestamp to look up.
        /// </param>
        /// <param name="timeout">
        ///     The maximum period of time the
        ///     call may block.
        /// </param>
        /// <returns>
        ///     A mapping from partition to the
        ///     timestamp and offset of the first message with
        ///     timestamp greater than or equal to the target
        ///     timestamp.
        /// </returns>
        /// <exception cref="T:Confluent.Kafka.KafkaException">
        ///     Thrown
        ///     if the operation fails.
        /// </exception>
        /// <exception cref="T:Confluent.Kafka.TopicPartitionOffsetException">
        ///     Thrown if any of the constituent results is
        ///     in error. The entire result (which may contain
        ///     constituent results that are not in error) is
        ///     available via the
        ///     <see cref="P:Confluent.Kafka.TopicPartitionOffsetException.Results" />
        ///     property of the exception.
        /// </exception>
        List<XXXTopicPartitionOffset> OffsetsForTimes(
            IEnumerable<XXXTopicPartitionTimestamp> timestampsToSearch,
            TimeSpan timeout);
    }
}
