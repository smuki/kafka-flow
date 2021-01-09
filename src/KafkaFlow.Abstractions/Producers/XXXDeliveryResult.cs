using System;
using System.Collections.Generic;
using System.Text;
using MessagePipeline.Consumers;
using MessagePipeline.Producers;

namespace MessagePipeline
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
