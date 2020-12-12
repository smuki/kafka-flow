using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Consumers
{
    /// <summary>
    ///     Represents a (deserialized) Kafka message.
    /// </summary>
    public class XXXMessage<TKey, TValue> : XXXMessageMetadata
    {
        /// <summary>
        ///     Gets the message key value (possibly null).
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        ///     Gets the message value (possibly null).
        /// </summary>
        public TValue Value { get; set; }
    }
}
