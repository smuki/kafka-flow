using KafkaFlow.Consumers;
using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow
{
    public class XXXDeliveryReport : XXXDeliveryResult
    {
        public XXXDeliveryReport(IMessageHeaders headers, byte[] Payload) : base(headers, Payload)
        {
        }
        public XXXError Error { get; set; }
        //
        // 摘要:
        //     The TopicPartitionOffsetError associated with the message.
        public XXXTopicPartitionOffset TopicPartitionOffset { get; set; }
    }
}
