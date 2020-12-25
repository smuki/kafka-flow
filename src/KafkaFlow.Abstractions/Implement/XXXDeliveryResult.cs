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
            this.Status = PersistenceStatus.NotPersisted;
        }
        /// <summary>
        //     The persistence status of the message
        /// </summary>
        public PersistenceStatus Status { get; set; }
    }
}
