using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Consumers
{
    /// <summary>
    ///     Encapsulates the result of a successful produce request.
    /// </summary>
    public class XXXDeliveryResult<TKey, TValue>
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
            => new XXXTopicPartition(Topic, Partition);

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
        ///     The Kafka message.
        /// </summary>
        public XXXMessage<TKey, TValue> Message { get; set; }

        /// <summary>
        ///     The Kafka message Key.
        /// </summary>
        public TKey Key
        {
            get { return Message.Key; }
            set { Message.Key = value; }
        }

        /// <summary>
        ///     The Kafka message Value.
        /// </summary>
        public TValue Value
        {
            get { return Message.Value; }
            set { Message.Value = value; }
        }

        /// <summary>
        ///     The Kafka message timestamp.
        /// </summary>
        public DateTimeOffset Timestamp
        {
            get { return Message.Timestamp; }
            set { Message.Timestamp = value; }
        }

        ///// <summary>
        /////     The Kafka message headers.
        ///// </summary>
        //public Headers Headers
        //{
        //    get { return Message.Headers; }
        //    set { Message.Headers = value; }
        //}
    }
}
