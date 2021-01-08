using MessagePipeline.Consumers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePipeline
{
    public class XXXDeliveryReport : XXXDeliveryResult
    {
        public XXXDeliveryReport(IMessageHeaders headers, byte[] Payload) : base(headers, Payload)
        {
        }
        public XXXError Error { get; set; }
    }
}
