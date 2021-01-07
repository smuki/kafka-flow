using System;
using System.Collections.Generic;
using System.Text;
using KafkaFlow.Consumers;
using KafkaFlow.Producers;

namespace KafkaFlow
{
    public class XXXDeliveryResult : IntermediateMessage
    {
        public XXXDeliveryResult(IMessageHeaders headers, byte[] Payload) : base(headers, Payload)
        {
            this.Status = XXXPersistenceStatus.NotPersisted;
        }
        public XXXPersistenceStatus Status { get; set; }
    }
}
