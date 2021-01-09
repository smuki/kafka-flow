using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePipeline.Consumers
{
    /// <summary>
    ///     Represents a message consumed from a Kafka cluster.
    /// </summary>
    public class XXXConsumeResult<TKey, TValue>
    {
        /// <summary>
        ///     The topic associated with the message.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        ///     The partition associated with the message.
        /// </summary>
        public int Partition { get; set; }

        /// <summary>
        ///     The partition offset associated with the message.
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        ///     The TopicPartition associated with the message.
        /// </summary>
        public XXXTopicPartition TopicPartition
        {
            get
            {
                return new XXXTopicPartition(Topic, Partition);
            }
        }
        /// <summary>
        ///     The TopicPartitionOffset associated with the message.
        /// </summary>
        public XXXTopicPartitionOffset TopicPartitionOffset
        {
            get
            {
                return new XXXTopicPartitionOffset(Topic, Partition, Offset);
            }
            set
            {
                Topic = value.Topic;
                Partition = value.Partition;
                Offset = value.Offset;
            }
        }
        /// <summary>
        ///     The Kafka message, or null if this ConsumeResult
        ///     instance represents an end of partition event.
        /// </summary>
        public XXXMessage<TKey, TValue> Message { get; set; }

        /// <summary>
        ///     True if this instance represents an end of partition
        ///     event, false if it represents a message in kafka.
        /// </summary>
        public bool IsPartitionEOF { get; set; }
    }
}
