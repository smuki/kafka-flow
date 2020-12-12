using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Consumers
{
    /// <summary>
    /// Intermediate Message
    /// </summary>
    public class IntermediateMessage
    {
        public IntermediateMessage(IMessageHeaders headers, byte[] Payload)
        {
            this.Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            this.Payload = Payload;
            this.Timestamp = DateTimeOffset.UtcNow;
        }
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        ///     The topic associated with the message.
        /// </summary>
        public string Topic { get; set; }
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
        ///     The partition associated with the message.
        /// </summary>
        public int Partition { get; set; }
        /// <summary>
        ///     The partition offset associated with the message.
        /// </summary>
        public long Offset { get; set; }
        public IMessageHeaders Headers { get; }
        public string Key { get; set; }
        /// <summary>
        /// Gets the body object of this message
        /// </summary>
        public byte[] Payload { get; }
    }
}
